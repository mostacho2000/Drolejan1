using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[DisallowMultipleComponent]
public class PlayerController2D : MonoBehaviour
{
    // =========================
    //  Referencias
    // =========================
    [Header("Refs")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private Animator anim;

    // =========================
    //  Movimiento
    // =========================
    [Header("Movimiento")]
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float acceleration = 60f;
    [SerializeField] private float deceleration = 70f;

    // =========================
    //  Salto
    // =========================
    [Header("Salto")]
    [SerializeField] private float jumpForce = 12f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = .1f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private int maxJumps = 2;
    [SerializeField, Range(.2f, 1.2f)] private float secondJumpMultiplier = .85f;
    [SerializeField, Range(.3f, 1f)] private float jumpCutMultiplier = .6f; // suelta botón para salto corto

    private int jumpsRemaining;
    private bool grounded;

    // =========================
    //  Disparo (bala)
    // =========================
    [Header("Disparo (bala)")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float fireRate = 0.5f; // segundos entre tiros
    [SerializeField] private bool autoFire = false;
    private float fireTimer;

    // =========================
    //  Granada (opcional)
    // =========================
    [Header("Granada (opcional)")]
    [SerializeField] private Transform throwPoint;
    [SerializeField] private GameObject grenadePrefab;
    [SerializeField] private float throwForce = 10f;
    [SerializeField] private float upwardForce = 6f;
    [SerializeField] private float grenadeCooldown = 0.6f;
    private float grenadeTimer;

    // =========================
    //  Animator params
    // =========================
    private static readonly int HASH_SPEED = Animator.StringToHash("Speed");
    private static readonly int HASH_ISGROUNDED = Animator.StringToHash("IsGrounded");
    private static readonly int HASH_SHOOT = Animator.StringToHash("Shoot");

    private bool hasSpeedParam, hasIsGroundedParam, hasShootParam;

    // =========================
    //  Input System refs
    // (arrastra desde tu Input Actions)
    // =========================
    [Header("Inputs (Input System)")]
    public InputActionReference moveRef;
    public InputActionReference jumpRef;
    public InputActionReference fireRef;
    public InputActionReference grenadeRef;

    private InputAction moveAction, jumpAction, fireAction, grenadeAction;

    // =========================
    //  Ciclo de vida
    // =========================
    private void Awake()
    {
        if (!rb) rb = GetComponent<Rigidbody2D>();
        if (!sr) sr = GetComponentInChildren<SpriteRenderer>();
        if (!anim) anim = GetComponent<Animator>();

        // Detecta si el Animator tiene esos parámetros
        if (anim)
        {
            foreach (var p in anim.parameters)
            {
                if (p.nameHash == HASH_SPEED) hasSpeedParam = true;
                else if (p.nameHash == HASH_ISGROUNDED) hasIsGroundedParam = true;
                else if (p.nameHash == HASH_SHOOT) hasShootParam = true;
            }
        }
    }

    private void OnEnable()
    {
        moveAction = moveRef ? moveRef.action : null;
        jumpAction = jumpRef ? jumpRef.action : null;
        fireAction = fireRef ? fireRef.action : null;
        grenadeAction = grenadeRef ? grenadeRef.action : null;

        moveAction?.Enable();
        jumpAction?.Enable();
        fireAction?.Enable();
        grenadeAction?.Enable();

        jumpsRemaining = maxJumps;
    }

    private void OnDisable()
    {
        moveAction?.Disable();
        jumpAction?.Disable();
        fireAction?.Disable();
        grenadeAction?.Disable();
    }

    private void Update()
    {
        // Timers
        fireTimer -= Time.deltaTime;
        grenadeTimer -= Time.deltaTime;

        // Ground check
        grounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        if (grounded) jumpsRemaining = maxJumps;

        // Anim
        if (anim)
        {
            if (hasIsGroundedParam) anim.SetBool(HASH_ISGROUNDED, grounded);
            if (hasSpeedParam) anim.SetFloat(HASH_SPEED, Mathf.Abs(GetVel().x));
        }

        // Saltar
        if (jumpAction != null && jumpAction.WasPressedThisFrame())
            TryJump();

        // Disparo
        if (fireAction != null)
        {
            bool pressed = fireAction.WasPressedThisFrame();
            bool holding = fireAction.IsPressed();

            if (fireTimer <= 0f && ((autoFire && holding) || pressed))
                Shoot();
        }

        // Granada (opcional + GameManager granadas)
        if (grenadeAction != null && grenadeAction.WasPressedThisFrame())
            ThrowGrenade();

        // Flip mirando
        HandleFlipByInput();
        // Movimiento en Update -> mejor pasarlo a FixedUpdate para físicas, aquí solo inputs
    }

    private void FixedUpdate()
    {
        // Movimiento suavizado
        float target = ReadMoveX() * moveSpeed;
        float current = GetVel().x;
        float accel = Mathf.Abs(target) > 0.01f ? acceleration : deceleration;
        float newX = Mathf.MoveTowards(current, target, accel * Time.fixedDeltaTime);
        SetVel(new Vector2(newX, GetVel().y));

        // Si suelta salto y va subiendo, corta
        if (jumpAction != null && jumpAction.WasReleasedThisFrame() && GetVel().y > 0f)
        {
            SetVel(new Vector2(GetVel().x, GetVel().y * jumpCutMultiplier));
        }
    }

    // =========================
    //  Movimiento helpers
    // =========================
    float ReadMoveX()
    {
        if (moveAction == null) return 0f;

        // soporta bindings de Vector2 o 1D
        if (moveAction.expectedControlType == "Vector2")
        {
            Vector2 v = moveAction.ReadValue<Vector2>();
            return v.x;
        }
        else
        {
            return moveAction.ReadValue<float>();
        }
    }

    void HandleFlipByInput()
    {
        float x = ReadMoveX();
        if (Mathf.Abs(x) > 0.01f && sr)
            sr.flipX = x < 0f;
    }

    // =========================
    //  Salto
    // =========================
   public void TryJump()
    {
        if (jumpsRemaining <= 0) return;

        float force = jumpForce;
        if (jumpsRemaining < maxJumps) // segundo salto
            force *= secondJumpMultiplier;

        SetVel(new Vector2(GetVel().x, 0f));
        AddVel(new Vector2(0f, force));
        jumpsRemaining--;
    }

    // =========================
    //  Disparo
    // =========================
    public void Shoot()
    {
        if (!bulletPrefab || !firePoint) return;

        // instanciar bala
        var go = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);

        // dirección según flip
        var dir = sr && sr.flipX ? Vector2.left : Vector2.right;

        // velocidad a la bala si tiene rigidbody
        var brb = go.GetComponent<Rigidbody2D>();
        if (brb)
        {
#if UNITY_6000_0_OR_NEWER
            brb.linearVelocity = dir * 18f; // o la velocidad interna de tu bala
#else
            brb.velocity = dir * 18f;
#endif
        }

        // rotar visual si quieres
        if (dir.x < 0f) go.transform.localScale = new Vector3(-go.transform.localScale.x, go.transform.localScale.y, go.transform.localScale.z);

        fireTimer = fireRate;

        // anim
        if (anim && hasShootParam)
        {
            anim.SetBool(HASH_SHOOT, true);
            // lo apagamos en un frame
            StartCoroutine(ResetShootBoolNextFrame());
        }
    }

    System.Collections.IEnumerator ResetShootBoolNextFrame()
    {
        yield return null;
        if (anim && hasShootParam) anim.SetBool(HASH_SHOOT, false);
    }

    // =========================
    //  Granada
    // =========================
    void ThrowGrenade()
    {
        if (grenadeTimer > 0f) return;
        if (!grenadePrefab || !throwPoint) return;

        // Si manejas granadas con GameManager:
        if (GameManager.instancia && !GameManager.instancia.TieneGranadas())
            return;

        var go = Instantiate(grenadePrefab, throwPoint.position, Quaternion.identity);
        var grb = go.GetComponent<Rigidbody2D>();
        if (grb)
        {
            var dir = sr && sr.flipX ? -1f : 1f;
#if UNITY_6000_0_OR_NEWER
            grb.AddForce(new Vector2(dir * throwForce, upwardForce), ForceMode2D.Impulse);
#else
            grb.AddForce(new Vector2(dir * throwForce, upwardForce), ForceMode2D.Impulse);
#endif
        }

        grenadeTimer = grenadeCooldown;
        GameManager.instancia?.CambiarGranadas(-1);
    }

    // =========================
    //  Daño (para que enemigos te golpeen)
    // =========================
    public void ApplyDamage(int amount)
    {
        int dmg = Mathf.Abs(amount);
        GameManager.instancia?.CambiarVidas(-dmg);
        // Aquí puedes lanzar animación "Hurt" si la tienes
        // anim.SetTrigger("Hurt");
    }

    // =========================
    //  Cambiar parámetros de arma (usado por PlayerSkinSwitcher)
    // =========================
    /// <summary>
    /// Llamado por PlayerSkinSwitcher para cambiar el prefab de bala y el fire rate
    /// según el arma seleccionada.
    /// </summary>
    public void SetWeaponParams(GameObject newBulletPrefab, float newFireRate)
    {
        if (newBulletPrefab) bulletPrefab = newBulletPrefab;
        fireRate = newFireRate;
    }

    // =========================
    //  Helpers Rigidbody (Unity 6 usa linearVelocity)
    // =========================
    Vector2 GetVel()
    {
#if UNITY_6000_0_OR_NEWER
        return rb.linearVelocity;
#else
        return rb.velocity;
#endif
    }

    void SetVel(Vector2 v)
    {
#if UNITY_6000_0_OR_NEWER
        rb.linearVelocity = v;
#else
        rb.velocity = v;
#endif
    }

    void AddVel(Vector2 dv)
    {
#if UNITY_6000_0_OR_NEWER
        rb.linearVelocity += dv;
#else
        rb.velocity += dv;
#endif
    }

    // =========================
    //  Gizmos
    // =========================
    private void OnDrawGizmosSelected()
    {
        if (groundCheck)
        {
            Gizmos.color = grounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
        if (firePoint)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(firePoint.position, firePoint.position + Vector3.right * .3f);
        }
    }
}
