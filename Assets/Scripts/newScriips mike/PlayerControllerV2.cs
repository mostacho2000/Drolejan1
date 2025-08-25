using System.Collections;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
public class PlayerControllerV2 : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private GameManager gameManager;      // opcional: asigna tu instancia
    [SerializeField] private TimeControler timeController; // si lo usas
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator anim;
    [SerializeField] private Transform bulletSpawn;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private TextMeshProUGUI scoreText;

    [Header("Prefabs")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private GameObject grenadePrefab;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float acceleration = 50f;
    [SerializeField] private float deceleration = 60f;
    [SerializeField] private float maxFallSpeed = -20f;

    [Header("Jump")]
    [SerializeField] private float jumpImpulse = 12f;
    [SerializeField] private int extraJumps = 1;          // 1 = doble salto
    [SerializeField] private float coyoteTime = 0.12f;
    [SerializeField] private float jumpBuffer = 0.12f;
    [SerializeField] private float jumpCutMultiplier = 0.5f;

    [Header("Ground Check")]
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float groundCheckRadius = 0.15f;

    [Header("Shooting")]
    [SerializeField] private float bulletForce = 20f;
    [SerializeField] private float bulletCooldown = 0.6f;
    [SerializeField] private float grenadeForce = 10f;

    [Header("Score")]
    [SerializeField] private int puntos = 0;

    // --- Animator hashes ---
    private static readonly int RUN = Animator.StringToHash("RUN");
    private static readonly int GROUND = Animator.StringToHash("ground");
    private static readonly int ATTACK = Animator.StringToHash("attack");
    private static readonly int SPEEDX = Animator.StringToHash("speedX");
    private static readonly int VELY = Animator.StringToHash("velocityY");

    // Estados
    private bool ready;
    private bool isGrounded;
    private int jumpsRemaining;
    private float coyoteCounter;
    private float jumpBufferCounter;
    private bool jumpHeld;
    private bool jumpPressed;
    private float moveInput; // -1..1
    private bool firePressed;
    private bool grenadePressed;
    private bool canShoot = true;

    private void Reset()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    private void Awake()
    {
        if (!rb) rb = GetComponent<Rigidbody2D>();
        if (!anim) anim = GetComponent<Animator>();
        jumpsRemaining = extraJumps;
    }

    private void Start()
    {
        // Si usas singleton: if (!gameManager) gameManager = GameManager.instancia;
        ready = true;
        UpdateScoreUI();
    }

    private void Update()
    {
        if (!ready || Time.timeScale <= 0f) return;

        // === INPUT (actual: sistema viejo; luego mapeamos al New Input System aquí) ===
        ReadInput();

        // Timers salto
        if (jumpPressed) jumpBufferCounter = jumpBuffer;
        else jumpBufferCounter -= Time.unscaledDeltaTime;

        if (isGrounded) coyoteCounter = coyoteTime;
        else coyoteCounter -= Time.unscaledDeltaTime;

        // Flip
        if (Mathf.Abs(moveInput) > 0.01f)
            transform.localScale = new Vector3(moveInput > 0 ? 1 : -1, 1, 1);

        // Animator params
        anim.SetBool(RUN, Mathf.Abs(moveInput) > 0.05f);
        anim.SetBool(GROUND, isGrounded);
        anim.SetFloat(SPEEDX, Mathf.Abs(rb.linearVelocity.x));
        anim.SetFloat(VELY, rb.linearVelocity.y);

        TryJump(); // solo intención aquí

        if (firePressed) TryShoot();
        if (grenadePressed) TryThrowGrenade();
    }

    private void FixedUpdate()
    {
        // Ground check
        if (groundCheck)
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundMask);
        else
            isGrounded = true;

        // Movimiento con aceleración
        float targetSpeed = moveInput * moveSpeed;
        float speedDiff = targetSpeed - rb.linearVelocity.x;
        float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? acceleration : deceleration;
        float force = accelRate * speedDiff;

        rb.AddForce(new Vector2(force, 0f), ForceMode2D.Force);

        // Limitar caída
        if (rb.linearVelocity.y < maxFallSpeed)
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, maxFallSpeed);
    }

    // ---------------- INPUT (viejo) ----------------
    private void ReadInput()
    {
        moveInput = Input.GetAxisRaw("Horizontal");

        if (Input.GetButtonDown("Jump")) { jumpPressed = true; jumpHeld = true; }
        if (Input.GetButtonUp("Jump"))   { jumpHeld = false; }

        firePressed    = Input.GetButtonDown("Fire1");
        grenadePressed = Input.GetButtonDown("Fire2");
    }

    // ---------------- SALTO ----------------
    private void TryJump()
    {
        bool canGroundJump = coyoteCounter > 0f;
        bool canAirJump = (!isGrounded && jumpsRemaining > 0);

        if (jumpBufferCounter > 0f && (canGroundJump || canAirJump))
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            rb.AddForce(Vector2.up * jumpImpulse, ForceMode2D.Impulse);

            // consumir recursos
            if (canGroundJump)
            {
                coyoteCounter = 0f;
                jumpsRemaining = extraJumps;
            }
            else if (canAirJump)
            {
                jumpsRemaining--;
            }

            jumpBufferCounter = 0f;
        }

        // Variable jump: cortar si suelta
        if (!jumpHeld && rb.linearVelocity.y > 0f)
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * jumpCutMultiplier);

        if (isGrounded) jumpsRemaining = extraJumps;
    }

    // ---------------- DISPARO ----------------
    private void TryShoot()
    {
        if (!canShoot || bulletPrefab == null || bulletSpawn == null) return;

        anim.SetTrigger(ATTACK);

        GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position, Quaternion.identity);
        if (bullet.TryGetComponent<Rigidbody2D>(out var rbBullet))
        {
            float dir = Mathf.Sign(transform.localScale.x);
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

    // ---------------- GRANADA ----------------
    private void TryThrowGrenade()
    {
        if (grenadePrefab == null) return;
        if (gameManager != null && gameManager.numGranadas <= 0) return;

        GameObject g = Instantiate(grenadePrefab, transform.position, Quaternion.identity);
        if (g.TryGetComponent<Rigidbody2D>(out var rbG))
        {
            float dir = Mathf.Sign(transform.localScale.x);
            rbG.AddForce(Vector2.right * grenadeForce * dir, ForceMode2D.Impulse);
        }

        if (gameManager != null) gameManager.CambiarGranadas();
    }

    // ---------------- COLISIONES ----------------
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("balaMuerte"))
            gameManager?.CambiarVidas();

        if (collision.gameObject.CompareTag("aguita") ||
            collision.gameObject.CompareTag("BALAFINAL"))
            GameManager.instancia?.GameOverr();
    }

    // ---------------- UTILS ----------------
    private void OnDrawGizmosSelected()
    {
        if (!groundCheck) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }

    private void UpdateScoreUI()
    {
        if (scoreText) scoreText.text = $"Puntos: {puntos}";
    }

    // === AnimationEvent hook (opcional) ===
    // En tu clip de DISPARANDO puedes poner un Animation Event que llame a este método en el frame del disparo:
    public void Shoot_FromAnimationEvent()
    {
        TryShoot();
    }
}
