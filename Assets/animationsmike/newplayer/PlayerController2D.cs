using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController2D : MonoBehaviour
{
    // ---------- Referencias ----------
    [Header("Refs")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private Animator anim;

    [Tooltip("Padre del arma. Al invertir su escala X también se invierte el firePoint.")]
    [SerializeField] private Transform weaponPivot;
    [SerializeField] private Transform firePoint;

    // ---------- Movimiento ----------
    [Header("Movimiento")]
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float acceleration = 60f;
    [SerializeField] private float deceleration = 70f;

    // ---------- Salto ----------
    [Header("Salto")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = .12f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float jumpForce = 12f;
    [SerializeField] private int maxJumps = 2;
    [SerializeField, Range(.3f, 1f)] private float jumpCutMultiplier = .6f;

    private bool grounded;
    private int jumpsLeft;

    // ---------- Disparo ----------
    [Header("Disparo")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float bulletSpeed = 18f;
    [SerializeField] private float fireRate = 0.25f;
    private float fireTimer;

    // ---------- Granada (opcional) ----------
    [Header("Granada (opcional)")]
    [SerializeField] private Transform throwPoint;
    [SerializeField] private Grenade2D grenadePrefab;
    [SerializeField] private float throwForce = 10f;
    [SerializeField] private float upwardForce = 6f;
    [SerializeField] private float grenadeCooldown = .6f;
    private float grenadeTimer;

    // ---------- Input (New Input System) ----------
    [Header("Inputs (New Input System)")]
    public InputActionReference moveRef;
    public InputActionReference jumpRef;
    public InputActionReference fireRef;
    public InputActionReference grenadeRef;

    private InputAction moveA, jumpA, fireA, grenadeA;

    // ---------- Estado ----------
    private bool facingRight = true; // true = derecha, false = izquierda

    // ---------- Animator (opcionales) ----------
    private static readonly int HASH_SPEED     = Animator.StringToHash("Speed");
    private static readonly int HASH_GROUNDED  = Animator.StringToHash("IsGrounded");
    private static readonly int HASH_SHOOT     = Animator.StringToHash("Shoot");

    // ============================= CICLO DE VIDA =============================
    private void Awake()
    {
        if (!rb) rb = GetComponent<Rigidbody2D>();
        if (!sr) sr = GetComponentInChildren<SpriteRenderer>();
        if (!anim) anim = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        moveA    = moveRef    ? moveRef.action    : null;
        jumpA    = jumpRef    ? jumpRef.action    : null;
        fireA    = fireRef    ? fireRef.action    : null;
        grenadeA = grenadeRef ? grenadeRef.action : null;

        moveA?.Enable();
        jumpA?.Enable();
        fireA?.Enable();
        grenadeA?.Enable();

        jumpsLeft = maxJumps;
    }

    private void OnDisable()
    {
        moveA?.Disable();
        jumpA?.Disable();
        fireA?.Disable();
        grenadeA?.Disable();
    }

    private void Update()
    {
        fireTimer    -= Time.deltaTime;
        grenadeTimer -= Time.deltaTime;

        // Ground check
        if (groundCheck)
            grounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        if (grounded) jumpsLeft = maxJumps;

        // Animación básica
        if (anim)
        {
            anim.SetBool(HASH_GROUNDED, grounded);
            anim.SetFloat(HASH_SPEED, Mathf.Abs(Vel.x));
        }

        // Saltar
        if (jumpA != null && jumpA.WasPressedThisFrame())
            TryJump();

        // Disparo
        if (fireA != null && fireA.WasPressedThisFrame() && fireTimer <= 0f)
            Shoot();

        // Granada
        if (grenadeA != null && grenadeA.WasPressedThisFrame())
            ThrowGrenade();

        // Mirar según movimiento (y recordar último lado)
        float mx = ReadMoveX();
        if (Mathf.Abs(mx) > 0.01f)
            SetFacing(mx > 0f);
    }

    private void FixedUpdate()
    {
        // Aceleración / frenado
        float target  = ReadMoveX() * moveSpeed;
        float current = Vel.x;
        float accel   = Mathf.Abs(target) > 0.01f ? acceleration : deceleration;
        float newX    = Mathf.MoveTowards(current, target, accel * Time.fixedDeltaTime);
        Vel = new Vector2(newX, Vel.y);

        // Cortar salto si suelta botón en ascenso
        if (jumpA != null && jumpA.WasReleasedThisFrame() && Vel.y > 0f)
            Vel = new Vector2(Vel.x, Vel.y * jumpCutMultiplier);
    }

    // ============================= MOVIMIENTO =============================
    // ============================= MOVIMIENTO =============================
private float ReadMoveX()
{
    if (moveA == null) return 0f;

    // Si la acción es un Vector2 (por ejemplo stick del gamepad)
    if (moveA.expectedControlType == "Vector2")
    {
        return moveA.ReadValue<Vector2>().x;
    }

    // Si es un eje 1D (Axis, como A/D, flechas, etc.)
    return moveA.ReadValue<float>();
}


    private void SetFacing(bool toRight)
    {
        facingRight = toRight;

        // 1) Voltear sprite del personaje
        if (sr) sr.flipX = !facingRight;

        // 2) Mover y voltear el weaponPivot (arma + firePoint)
        if (weaponPivot)
        {
            // Mover el pivot al lado correcto manteniendo su distancia en X
            var pos = weaponPivot.localPosition;
            float absX = Mathf.Abs(pos.x);
            pos.x = absX * (facingRight ? 1f : -1f);
            weaponPivot.localPosition = pos;

            // Ajustar escala para que el arma/fuego apunten correctamente
            var s = weaponPivot.localScale;
            s.x = facingRight ? 1f : -1f;
            weaponPivot.localScale = s;
        }
    }

    private void TryJump()
    {
        if (jumpsLeft <= 0) return;
        jumpsLeft--;
        Vel = new Vector2(Vel.x, 0f);
        Vel += Vector2.up * jumpForce;
    }

    // ============================= DISPARO (IZQ/DER) =============================
    private void Shoot()
    {
        if (!bulletPrefab || !firePoint) return;

        Vector2 dir = facingRight ? Vector2.right : Vector2.left;

        var go = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        go.transform.right = dir; // orientar sprite de la bala

        var brb = go.GetComponent<Rigidbody2D>();
        if (brb)
        {
#if UNITY_6000_0_OR_NEWER
            brb.linearVelocity = dir * bulletSpeed;
#else
            brb.velocity = dir * bulletSpeed;
#endif
        }

        fireTimer = fireRate;

        if (anim) { anim.SetBool(HASH_SHOOT, true); StartCoroutine(ResetShootBool()); }
    }

    private System.Collections.IEnumerator ResetShootBool()
    {
        yield return null;
        if (anim) anim.SetBool(HASH_SHOOT, false);
    }

    // ============================= GRANADA (opcional) =============================
    // ============================= GRANADA (opcional) =============================
private void ThrowGrenade()
{
    // cooldown o falta de prefab/punto de lanzamiento
    if (grenadeTimer > 0f || !grenadePrefab || !throwPoint) return;

    // si hay GameManager y NO tiene granadas, no lanza
    if (GameManager.instancia != null && !GameManager.instancia.TieneGranadas)
        return;

    // dirección según hacia dónde está mirando el player
    float x = facingRight ? 1f : -1f;

    var go  = Instantiate(grenadePrefab, throwPoint.position, Quaternion.identity);
    var grb = go.GetComponent<Rigidbody2D>();
    if (grb)
        grb.AddForce(new Vector2(x * throwForce, upwardForce), ForceMode2D.Impulse);

    // cooldown
    grenadeTimer = grenadeCooldown;

    // 🔴 aquí avisamos al GameManager para que actualice contador + UI
    GameManager.instancia?.CambiarGranadas(-1);
}



    // ============================= API para cambiar arma =============================
    // Llamado por PlayerSkinSwitcher
    public void SetWeaponParams(GameObject newBulletPrefab, float newFireRate)
    {
        if (newBulletPrefab) bulletPrefab = newBulletPrefab;
        fireRate = newFireRate;
    }

    // Overload opcional (si tu SkinSwitcher pasa 3 parámetros)
    public void SetWeaponParams(GameObject newBulletPrefab, float newFireRate, float newBulletSpeed)
    {
        if (newBulletPrefab) bulletPrefab = newBulletPrefab;
        fireRate    = newFireRate;
        bulletSpeed = newBulletSpeed;
    }

    // ============================= Helpers Rigidbody =============================
    private Vector2 Vel
    {
        get
        {
#if UNITY_6000_0_OR_NEWER
            return rb.linearVelocity;
#else
            return rb.velocity;
#endif
        }
        set
        {
#if UNITY_6000_0_OR_NEWER
            rb.linearVelocity = value;
#else
            rb.velocity = value;
#endif
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck)
        {
            Gizmos.color = grounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }

    // ============================= Getter público de dirección =============================
    // Para que otros scripts (como PlayerGrenade2D) sepan hacia dónde mira el player
    public bool IsFacingRight => facingRight;
}

