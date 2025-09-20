using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class PlayerController2D : MonoBehaviour
{
    [Header("Input Actions (drag & drop)")]
    [SerializeField] private InputActionReference moveRef;    // Move (Axis -1..1)
    [SerializeField] private InputActionReference jumpRef;    // Jump (Button)
    [SerializeField] private InputActionReference fireRef;    // Fire (Button)

    [Header("Movimiento")]
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float acceleration = 60f;
    [SerializeField] private float deceleration = 70f;
    [SerializeField, Range(0f, 1f)] private float inputDeadzone = 0.2f;

    [Header("Salto")]
    [SerializeField] private float jumpForce = 12f;
    [SerializeField, Range(0f, 1f)] private float jumpCutMultiplier = 0.5f;

    [Header("Suelo")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.12f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Coyote / Buffer")]
    [SerializeField] private float coyoteTime = 0.1f;
    [SerializeField] private float jumpBuffer = 0.1f;

    [Header("Salto múltiple")]
    [SerializeField] private int maxJumps = 2;                       // 2 = doble salto
    [SerializeField, Range(0.5f, 1.2f)] private float secondJumpMultiplier = 0.85f;

    [Header("Disparo (bala)")]
    [SerializeField] private Transform firePoint;                    // boca del arma
    [SerializeField] private Bullet2D bulletPrefab;                  // tu prefab de bala
    [SerializeField] private float fireRate = 0.3333333f;            // 1/3 ≈ cooldown 3s (semi-auto)
    [SerializeField] private bool autoFire = false;                  // si true y fireRate > 0 = ráfaga

    [Header("Animator")]
    [SerializeField] private Animator anim;                          // usa parámetros, no nombres de estados
    [SerializeField] private float animSpeedThreshold = 0.2f;        // evita Run por ruido

    // ================= UI / GAME =================
    [Header("UI / Game Manager")]
    [SerializeField] private GameManager controlador;                // se rellena en Inspector o por fallback
    [SerializeField] private TimeControler tiempo;                   // opcional (para sumar tiempo por moneda)
    [SerializeField] private TextMeshProUGUI textoScore;             // opcional si quieres que el Player lo toque
    [SerializeField] private Transform respawnPoint;                 // opcional
    [SerializeField] private int puntos = 0;                         // marcador local si lo usas
    [SerializeField] private int coinScoreValue = 1;
    [SerializeField] private int coinTimeBonus = 2;

    // Debug (solo lectura)
    [Header("Debug")]
    [SerializeField] private float dbgMoveInput;
    [SerializeField] private Vector2 dbgLinearVel;
    [SerializeField] private bool dbgIsGrounded;
    [SerializeField] private int dbgJumpCount;

    // Components
    private Rigidbody2D rb;
    private SpriteRenderer sr;

    // Input
    private InputAction moveAction, jumpAction, fireAction;

    // Runtime
    private float moveInput;
    private bool isGrounded;
    private float coyoteTimer;
    private float bufferTimer;
    private int jumpCount;
    private float fireCooldown;

    // Animator hashes
    private static readonly int HASH_SPEED      = Animator.StringToHash("Speed");
    private static readonly int HASH_ISGROUNDED = Animator.StringToHash("IsGrounded");
    private static readonly int HASH_SHOOT      = Animator.StringToHash("Shoot");
    private bool hasSpeedParam, hasIsGroundedParam, hasShootParam;

    private void Awake()
    {
        rb  = GetComponent<Rigidbody2D>();
        sr  = GetComponent<SpriteRenderer>();
        if (!anim) anim = GetComponent<Animator>();

        rb.freezeRotation = true;
        rb.interpolation  = RigidbodyInterpolation2D.Interpolate;

        if (groundLayer == 0) groundLayer = LayerMask.GetMask("Ground");

        // Cachea parámetros del Animator
        if (anim && anim.runtimeAnimatorController != null)
        {
            foreach (var p in anim.parameters)
            {
                if (p.nameHash == HASH_SPEED)      hasSpeedParam = true;
                if (p.nameHash == HASH_ISGROUNDED) hasIsGroundedParam = true;
                if (p.nameHash == HASH_SHOOT)      hasShootParam = true;
            }
        }

        // === UI/GameManager fallback (sin warnings deprecados) ===
#if UNITY_2023_1_OR_NEWER
        controlador ??= GameManager.instancia ?? Object.FindFirstObjectByType<GameManager>();
#else
        controlador ??= GameManager.instancia ?? FindObjectOfType<GameManager>();
#endif

        // (Opcional) inicializa texto local si lo tienes conectado
        if (textoScore) textoScore.text = "Puntos: " + puntos.ToString();
    }

    private void OnEnable()
    {
        moveAction = moveRef ? moveRef.action : null;
        jumpAction = jumpRef ? jumpRef.action : null;
        fireAction = fireRef ? fireRef.action : null;

        moveAction?.Enable();
        jumpAction?.Enable();
        fireAction?.Enable();
    }

    private void OnDisable()
    {
        moveAction?.Disable();
        jumpAction?.Disable();
        fireAction?.Disable();
    }

    private void Update()
    {
        if (moveAction == null || jumpAction == null) return;

        // ===== INPUT HORIZONTAL =====
        float raw = moveAction.ReadValue<float>();
        moveInput = (Mathf.Abs(raw) < inputDeadzone) ? 0f : raw;
        dbgMoveInput = moveInput;

        // Voltear sprite por input
        if (sr && Mathf.Abs(moveInput) > 0.01f)
            sr.flipX = moveInput < 0f;

        // ===== COYOTE / BUFFER / RESET SALTOS =====
        if (isGrounded) { coyoteTimer = coyoteTime; jumpCount = 0; }
        else            { coyoteTimer -= Time.deltaTime; }

        if (jumpAction.WasPressedThisFrame()) bufferTimer = jumpBuffer;
        else                                  bufferTimer -= Time.deltaTime;

        bool canExtraJump    = jumpCount < maxJumps;
        bool canGroundedJump = (isGrounded || coyoteTimer > 0f);

        if (bufferTimer > 0f && (canGroundedJump || canExtraJump))
        {
            DoJump();
            bufferTimer = 0f;
            coyoteTimer = 0f;
        }

        // Salto variable
        if (jumpAction.WasReleasedThisFrame() && rb.linearVelocity.y > 0f)
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * jumpCutMultiplier);

        // ===== DISPARO =====
        if (fireAction != null && bulletPrefab && firePoint)
        {
            if (fireCooldown > 0f) fireCooldown -= Time.deltaTime;

            bool pressedFrame = fireAction.WasPressedThisFrame();
            bool held         = fireAction.IsPressed();

            bool wantFire = (autoFire && fireRate > 0f)
                ? (held && fireCooldown <= 0f)           // ráfaga
                : (pressedFrame && fireCooldown <= 0f);  // 1 por clic

            if (wantFire)
            {
                fireCooldown = (fireRate > 0f) ? (1f / fireRate) : 0f;

                Fire(); // instancia bala

                // Trigger de anim (funciona en suelo/aire con tus transiciones)
                if (anim && anim.runtimeAnimatorController != null && hasShootParam)
                    anim.SetTrigger(HASH_SHOOT);
            }
        }

        // ===== ANIMATOR PARAMS =====
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
        if (groundCheck)
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
        Vector2 dir = (sr && sr.flipX) ? Vector2.left : Vector2.right;

        Bullet2D bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        bullet.Init(dir);

        // Ignorar colisión inicial con el Player
        var myCol     = GetComponent<Collider2D>();
        var bulletCol = bullet.GetComponent<Collider2D>();
        if (myCol && bulletCol) Physics2D.IgnoreCollision(myCol, bulletCol, true);
    }

    // ===================== UI / GAME HOOKS =====================

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Ejemplos de integración con tu GameManager/tags del proyecto:
        if (collision.gameObject.CompareTag("balaMuerte"))
            controlador?.CambiarVidas(-1);

        if (collision.gameObject.CompareTag("aguita") ||
            collision.gameObject.CompareTag("BALAFINAL"))
            GameManager.instancia?.GameOverr();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Monedas (puntos + tiempo)
        if (other.gameObject.CompareTag("coin"))
        {
            if (tiempo) tiempo.tiempoActual += coinTimeBonus;

            puntos += coinScoreValue;                        // local (si lo usas)
            if (textoScore) textoScore.text = "Puntos: " + puntos.ToString();

            GameManager.instancia?.CambiarPuntos(coinScoreValue);    // global UI
            Destroy(other.gameObject);
        }
    }

    /// Llama esto tras lanzar una granada para actualizar la UI del GameManager.
    public void NotifyGrenadeThrown()
    {
        GameManager.instancia?.CambiarGranadas(-1);
    }

    /// Si quieres validar que aún hay granadas antes de lanzar.
    public bool CanThrowGrenade()
    {
        return (GameManager.instancia == null) || GameManager.instancia.TieneGranadas();
    }

    private void OnDrawGizmosSelected()
    {
        if (!groundCheck) return;
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}
