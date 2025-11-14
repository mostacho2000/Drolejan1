using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class EnemyBullet2D : MonoBehaviour
{
    [Header("Ajustes")]
    [SerializeField] int damage = 1;
    [SerializeField] float lifeTime = 4f;      // segundos antes de autodestruirse
    [SerializeField] LayerMask hitMask = ~0;   // por si quieres filtrar capas (opcional)

    Rigidbody2D rb;
    Vector2 dir = Vector2.right;
    float speed = 10f;
    float life;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

#if UNITY_6000_0_OR_NEWER
        rb.bodyType = RigidbodyType2D.Kinematic;
#else
        rb.isKinematic = true;
#endif
        rb.freezeRotation = true;

        // El collider de la bala debe ser "Is Trigger"
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    /// <summary>
    /// Llamado por el jefe al crear la bala.
    /// </summary>
    public void Setup(Vector2 direction, float bulletSpeed)
    {
        dir = direction.sqrMagnitude > 0.0001f ? direction.normalized : Vector2.right;
        speed = Mathf.Max(0f, bulletSpeed);
    }

    void FixedUpdate()
    {
#if UNITY_6000_0_OR_NEWER
        rb.linearVelocity = dir * speed;
#else
        rb.velocity = dir * speed;
#endif
    }

    void Update()
    {
        life += Time.deltaTime;
        if (life >= lifeTime) Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Si definiste hitMask, respeta el filtro
        if (((1 << other.gameObject.layer) & hitMask) == 0)
            return;

        // Evita dañar a otros enemigos (opcional)
        if (other.CompareTag("Enemy") || other.CompareTag("Boss"))
            return;

        // Busca a alguien dañable en el objeto o su padre
        var dmg = other.GetComponentInParent<IDamageable>();
        if (dmg != null)
        {
            Vector2 hitPoint = transform.position;
            Vector2 hitDir   = dir;
            dmg.TakeDamage(damage, hitPoint, hitDir);
        }

        // Destruye siempre al impactar algo relevante
        Destroy(gameObject);
    }

    void OnBecameInvisible()
    {
        // Seguridad extra por si no colisiona
        Destroy(gameObject);
    }
}
