using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class PlayerController2D : MonoBehaviour
{
    [Header("Input Actions (drag & drop)")]
    [SerializeField] private InputActionReference moveRef;    // Move (Axis)
    [SerializeField] private InputActionReference jumpRef;    // Jump (Button)
    [SerializeField] private InputActionReference fireRef;    // Fire (Button)

    [Header("Movimiento")]
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float acceleration = 60f;
    [SerializeField] private float deceleration = 70f;
    [SerializeField] private float inputDeadzone = 0.2f;      // para stick

    [Header("Salto")]
    [SerializeField] private float jumpForce = 12f;
    [SerializeField, Range(0f, 1f)] private float jumpCutMultiplier = 0.5f;

    [Header("Suelo")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.1f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Coyote / Buffer")]
    [SerializeField] private float coyoteTime = 0.1f;
    [SerializeField] private float jumpBuffer = 0.1f;

    [Header("Salto múltiple")]
    [SerializeField] private int maxJumps = 2;                       // 2 = doble salto
    [SerializeField, Range(0.5f, 1.2f)] private float secondJumpMultiplier = 0.85f;

    [Header("Disparo")]
    [SerializeField] private Transform firePoint;                    // boca del arma
    [SerializeField] private Bullet2D bulletPrefab;                  // prefab de bala
    [SerializeField] private float fireRate = 10f;                   // balas por segundo (0 = semi)
    [SerializeField] private bool autoFire = true;                   // mantener para ráfaga

    [Header("Animator")]
    [SerializeField] private float animSpeedThreshold = 0.2f;        // evita saltar a Run por ruido

    // --- DEBUG en Inspector ---
    [Header("Debug")]
    [SerializeField] private float dbgMoveInput;
    [SerializeField] private Vector2 dbgLinearVel;
    [SerializeField] private bool dbgIsGrounded;
    [SerializeField] private int dbgJumpCount;
    [SerializeField] private bool dbgHasMoveAction, dbgHasJumpAction, dbgHasFireAction;

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Animator anim;

    // Input
    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction fireAction;

    // Runtime
    private float moveInput;
    private bool isGrounded;
    private float coyoteTimer;
    private float bufferTimer;
    private int jumpCount;
    private float fireCooldown;

    // Animator hashes
    private static readonly int HASH_SPEED       = Animator.StringToHash("Speed");
    private static readonly int HASH_ISGROUNDED  = Animator.StringToHash("IsGrounded");
    private static readonly int HASH_SHOOT       = Animator.StringToHash("Shoot");
    private bool hasSpeedParam, hasIsGroundedParam, hasShootParam;

    private void Awake()
    {
        rb  = GetComponent<Rigidbody2D>();
        sr  = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        rb.freezeRotation = true;
        rb.interpolation  = RigidbodyInterpolation2D.Interpolate;

        if (groundLayer == 0) groundLayer = LayerMask.GetMask("Ground");
    }

    private void OnEnable()
    {
        moveAction = moveRef  ? moveRef.action  : null;
        jumpAction = jumpRef  ? jumpRef.action  : null;
        fireAction = fireRef  ? fireRef.action  : null;

        dbgHasMoveAction = moveAction != null;
        dbgHasJumpAction = jumpAction != null;
        dbgHasFireAction = fireAction != null;

        moveAction?.Enable();
        jumpAction?.Enable();
        fireAction?.Enable();

        CacheAnimatorParams();
    }

    private void OnDisable()
    {
        moveAction?.Disable();
        jumpAction?.Disable();
        fireAction?.Disable();
    }

    private void CacheAnimatorParams()
    {
        hasSpeedParam = hasIsGroundedParam = hasShootParam = false;

        if (anim && anim.runtimeAnimatorController != null)
        {
            foreach (var p in anim.parameters)
            {
                if (p.nameHash == HASH_SPEED)      hasSpeedParam = true;
                if (p.nameHash == HASH_ISGROUNDED) hasIsGroundedParam = true;
                if (p.nameHash == HASH_SHOOT)      hasShootParam = true;
            }
        }
    }

    private void Update()
    {
        if (moveAction == null || jumpAction == null) return;

        // ====== INPUT ======
        float raw = moveAction.ReadValue<float>();
        if (Mathf.Abs(raw) < inputDeadzone) raw = 0f;   // deadzone para stick
        moveInput = raw;
        dbgMoveInput = moveInput;

        // Voltear sprite
        if (sr && Mathf.Abs(moveInput) > 0.01f)
            sr.flipX = moveInput < 0f;

        // ====== TIMERS / SALTO ======
        if (isGrounded)
        {
            coyoteTimer = coyoteTime;
            jumpCount = 0; // reset de saltos al tocar suelo
        }
        else coyoteTimer -= Time.deltaTime;

        if (jumpAction.WasPressedThisFrame()) bufferTimer = jumpBuffer;
        else                                   bufferTimer -= Time.deltaTime;

        bool canExtraJump    = jumpCount < maxJumps;
        bool canGroundedJump = (isGrounded || coyoteTimer > 0f);

        if (bufferTimer > 0f && (canGroundedJump || canExtraJump))
        {
            DoJump();
            bufferTimer = 0f;
            coyoteTimer = 0f;
        }

        // Salto variable (recorte)
        if (jumpAction.WasReleasedThisFrame() && rb.linearVelocity.y > 0f)
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * jumpCutMultiplier);

        // ====== DISPARO ======
        if (fireAction != null)
        {
            if (fireCooldown > 0f) fireCooldown -= Time.deltaTime;

            bool pressed = (autoFire && fireAction.IsPressed()) ||
                           (!autoFire && fireAction.WasPressedThisFrame());

            if (pressed && fireCooldown <= 0f)
            {
                Fire();
                fireCooldown = (fireRate > 0f) ? (1f / fireRate) : 0f;

                if (anim && anim.runtimeAnimatorController != null && hasShootParam)
                    anim.SetTrigger(HASH_SHOOT); // usa Trigger "Shoot" en tu Animator
            }
        }

        // ====== ANIMATOR ======
        if (anim && anim.runtimeAnimatorController != null)
        {
            float speedForAnim = Mathf.Abs(rb.linearVelocity.x);
            if (speedForAnim < animSpeedThreshold) speedForAnim = 0f;

            if (hasSpeedParam)      anim.SetFloat(HASH_SPEED,      speedForAnim);
            if (hasIsGroundedParam) anim.SetBool (HASH_ISGROUNDED, isGrounded);
        }

        // Debug
        dbgLinearVel  = rb.linearVelocity;
        dbgIsGrounded = isGrounded;
        dbgJumpCount  = jumpCount;
    }

    private void FixedUpdate()
    {
        // Ground check
        if (groundCheck != null)
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // Movimiento con aceleración/desaceleración
        float targetSpeed = moveInput * moveSpeed;
        float speedDiff   = targetSpeed - rb.linearVelocity.x;
        float accel       = (Mathf.Abs(targetSpeed) > 0.01f) ? acceleration : deceleration;
        float movement    = Mathf.Clamp(speedDiff * accel, -Mathf.Abs(accel), Mathf.Abs(accel)) * Time.fixedDeltaTime;

        rb.linearVelocity = new Vector2(rb.linearVelocity.x + movement, rb.linearVelocity.y);

        dbgLinearVel = rb.linearVelocity;
    }

    private void DoJump()
    {
        float force = (jumpCount == 0) ? jumpForce : (jumpForce * secondJumpMultiplier);

        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        rb.AddForce(Vector2.up * force, ForceMode2D.Impulse);

        jumpCount++;
        dbgJumpCount = jumpCount;
    }

    private void Fire()
    {
        if (bulletPrefab == null || firePoint == null) return;

        Vector2 dir = (sr != null && sr.flipX) ? Vector2.left : Vector2.right;

        Bullet2D bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        bullet.Init(dir);

        // Ignorar colisión con el propio Player
        var myCol     = GetComponent<Collider2D>();
        var bulletCol = bullet.GetComponent<Collider2D>();
        if (myCol && bulletCol) Physics2D.IgnoreCollision(myCol, bulletCol, true);
    }

    private void OnDrawGizmosSelected()
    {
        if (!groundCheck) return;
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}
