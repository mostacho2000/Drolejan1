using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator anim;
    [SerializeField] private Transform gfx;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Transform bulletSpawn;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private TextMeshProUGUI scoreText;

    [Header("Prefabs")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private GameObject grenadePrefab;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float acceleration = 50f;
    [SerializeField] private float deceleration = 60f;
    [SerializeField] private float maxFallSpeed = -22f;

    [Header("Jump")]
    [SerializeField] private float jumpImpulse = 12f;
    [SerializeField] private int extraJumps = 1;
    [SerializeField] private float coyoteTime = 0.12f;
    [SerializeField] private float jumpBuffer = 0.12f;
    [SerializeField] private float jumpCutMultiplier = 0.5f;

    [Header("Ground Check")]
    [SerializeField] private float groundCheckRadius = 0.16f;

    [Header("Combat")]
    [SerializeField] private float bulletForce = 20f;
    [SerializeField] private float bulletCooldown = 0.6f;
    [SerializeField] private float grenadeForce = 10f;

    [Header("Score")]
    [SerializeField] private int puntos = 0;

    // InputActionReferences — arrástralas desde Controls.inputactions
    [Header("Input Actions (drag from Controls.inputactions)")]
    public InputActionReference moveAction;
    public InputActionReference jumpAction;
    public InputActionReference shootAction;
    public InputActionReference grenadeAction;
    public InputActionReference pauseAction;

    // Animator hashes
    private static readonly int RUN = Animator.StringToHash("RUN");
    private static readonly int GROUND = Animator.StringToHash("ground");
    private static readonly int ATTACK = Animator.StringToHash("attack");
    private static readonly int SPEEDX = Animator.StringToHash("speedX");
    private static readonly int VELY = Animator.StringToHash("velocityY");

    // State
    private Vector2 moveVec;
    private bool isGrounded;
    private int jumpsRemaining;
    private float coyoteCounter;
    private float jumpBufferCounter;
    private bool jumpHeld;
    private bool firePressed;
    private bool grenadePressed;
    private bool canShoot = true;

    private void Reset()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        if (!gfx && transform.childCount > 0) gfx = transform.GetChild(0);
    }

    private void Awake()
    {
        if (!rb) rb = GetComponent<Rigidbody2D>();
        if (!anim) anim = GetComponent<Animator>();
        jumpsRemaining = extraJumps;
    }

    private void OnEnable()
    {
        // Enable actions + subscribe
        if (moveAction)    { moveAction.action.Enable();    moveAction.action.performed += OnMove; moveAction.action.canceled += OnMove; }
        if (jumpAction)    { jumpAction.action.Enable();    jumpAction.action.performed += OnJumpPerf; jumpAction.action.canceled += OnJumpCanc; }
        if (shootAction)   { shootAction.action.Enable();   shootAction.action.performed += OnShoot; }
        if (grenadeAction) { grenadeAction.action.Enable(); grenadeAction.action.performed += OnGrenade; }
        if (pauseAction)   { pauseAction.action.Enable();   pauseAction.action.performed += OnPause; }
    }

    private void OnDisable()
    {
        // Unsubscribe + disable
        if (moveAction)    { moveAction.action.performed -= OnMove; moveAction.action.canceled -= OnMove; moveAction.action.Disable(); }
        if (jumpAction)    { jumpAction.action.performed -= OnJumpPerf; jumpAction.action.canceled -= OnJumpCanc; jumpAction.action.Disable(); }
        if (shootAction)   { shootAction.action.performed -= OnShoot;   shootAction.action.Disable(); }
        if (grenadeAction) { grenadeAction.action.performed -= OnGrenade; grenadeAction.action.Disable(); }
        if (pauseAction)   { pauseAction.action.performed -= OnPause;   pauseAction.action.Disable(); }
    }

    private void Update()
    {
        // timers
        if (isGrounded) coyoteCounter = coyoteTime; else coyoteCounter -= Time.unscaledDeltaTime;
        if (jumpBufferCounter > 0f) jumpBufferCounter -= Time.unscaledDeltaTime;

        // flip gfx
        if (gfx && Mathf.Abs(moveVec.x) > 0.01f)
            gfx.localScale = new Vector3(moveVec.x > 0 ? 1 : -1, 1, 1);

        // animator params (usar linearVelocity)
        anim.SetBool(RUN, Mathf.Abs(rb.linearVelocity.x) > 0.1f && isGrounded);
        anim.SetBool(GROUND, isGrounded);
        anim.SetFloat(SPEEDX, Mathf.Abs(rb.linearVelocity.x));
        anim.SetFloat(VELY, rb.linearVelocity.y);

        TryJump();

        if (firePressed)   { TryShoot();   firePressed = false; }
        if (grenadePressed){ TryGrenade(); grenadePressed = false; }
    }

    private void FixedUpdate()
    {
        // ground check
        isGrounded = groundCheck
            ? Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundMask)
            : true;

        // movement with accel/decel (usar linearVelocity)
        float targetSpeed = moveVec.x * moveSpeed;
        float speedDiff = targetSpeed - rb.linearVelocity.x;
        float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? acceleration : deceleration;
        float force = accelRate * speedDiff;
        rb.AddForce(new Vector2(force, 0f), ForceMode2D.Force);

        // clamp fall
        if (rb.linearVelocity.y < maxFallSpeed)
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, maxFallSpeed);
    }

    // ===== Input callbacks =====
    private void OnMove(InputAction.CallbackContext ctx) => moveVec = ctx.ReadValue<Vector2>();
    private void OnJumpPerf(InputAction.CallbackContext ctx) { jumpHeld = true; jumpBufferCounter = jumpBuffer; }
    private void OnJumpCanc(InputAction.CallbackContext ctx) { jumpHeld = false; }
    private void OnShoot(InputAction.CallbackContext ctx)    { firePressed = true; }
    private void OnGrenade(InputAction.CallbackContext ctx)  { grenadePressed = true; }
    private void OnPause(InputAction.CallbackContext ctx)    { /* opcional: pausa */ }

    // ===== Jump =====
    private void TryJump()
    {
        bool canGroundJump = coyoteCounter > 0f;
        bool canAirJump = (!isGrounded && jumpsRemaining > 0);

        if (jumpBufferCounter > 0f && (canGroundJump || canAirJump))
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            rb.AddForce(Vector2.up * jumpImpulse, ForceMode2D.Impulse);

            if (canGroundJump) { coyoteCounter = 0f; jumpsRemaining = extraJumps; }
            else if (canAirJump) { jumpsRemaining--; }

            jumpBufferCounter = 0f;
        }

        if (!jumpHeld && rb.linearVelocity.y > 0f)
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * jumpCutMultiplier);

        if (isGrounded) jumpsRemaining = extraJumps;
    }

    // ===== Shooting =====
    private void TryShoot()
    {
        if (!canShoot || !bulletPrefab || !bulletSpawn) return;

        anim.SetTrigger(ATTACK);

        GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position, Quaternion.identity);
        if (bullet.TryGetComponent<Rigidbody2D>(out var rbBullet))
        {
            float dir = gfx ? Mathf.Sign(gfx.localScale.x) : Mathf.Sign(transform.localScale.x);
            rbBullet.AddForce(Vector2.right * bulletForce * dir, ForceMode2D.Impulse);
        }
        StartCoroutine(BulletCooldown());
    }

    private IEnumerator BulletCooldown()
    {
        canShoot = false;
        yield return new WaitForSeconds(bulletCooldown);
        canShoot = true;
    }

    // ===== Grenade =====
    private void TryGrenade()
    {
        if (!grenadePrefab) return;

        GameObject g = Instantiate(grenadePrefab, transform.position, Quaternion.identity);
        if (g.TryGetComponent<Rigidbody2D>(out var rbG))
        {
            float dir = gfx ? Mathf.Sign(gfx.localScale.x) : Mathf.Sign(transform.localScale.x);
            rbG.AddForce(Vector2.right * grenadeForce * dir, ForceMode2D.Impulse);
        }
    }

    // Animation Event hook
    public void Shoot_FromAnimationEvent() => TryShoot();

    private void OnDrawGizmosSelected()
    {
        if (!groundCheck) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}
