using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class Bullet2D : MonoBehaviour
{
    [Header("Movimiento / Vida")]
    [SerializeField] float speed = 18f;
    [SerializeField] float lifeTime = 2f;

    [Header("Daño")]
    [SerializeField] int damage = 1;
    [SerializeField] LayerMask hitMask; // asigna Enemy y Ground (si quieres que se destruya al tocar suelo)

    Rigidbody2D rb;
    Vector2 moveDir = Vector2.right;

    // Llama esto al instanciar (desde el Player)
    public void Init(Vector2 dir)
    {
        moveDir = dir.sqrMagnitude > 0.001f ? dir.normalized : Vector2.right;
        if (rb) rb.linearVelocity = moveDir * speed;
        // orientar sprite (opcional)
        transform.right = moveDir;
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;

        // Asegura que el collider es trigger (para OnTriggerEnter2D)
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    void OnEnable() => Destroy(gameObject, lifeTime);

    void FixedUpdate()
    {
        // por si algo cambia la velocidad
        if (rb) rb.linearVelocity = moveDir * speed;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // filtra por máscara (Enemy, Ground, etc.)
        if (hitMask != 0 && ((1 << other.gameObject.layer) & hitMask) == 0) return;

        // daño si el objetivo lo soporta
        var target = other.GetComponent<IDamageable>();
        if (target != null)
        {
            target.TakeDamage(damage, transform.position, moveDir);
            Destroy(gameObject);
            return;
        }

        // si tocó algo de la máscara (ej. Ground) y no es IDamageable, también se destruye
        Destroy(gameObject);
    }
}
