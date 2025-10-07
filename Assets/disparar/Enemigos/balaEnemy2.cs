using UnityEngine;

public class EnemyBullet2D : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] float speed = 15f;
    [SerializeField] float lifeTime = 4f;
    [SerializeField] bool faceVelocity = true;

    [Header("Daño")]
    [SerializeField] int damage = 1;
    [Tooltip("Opcional: si lo pones, solo reaccionará a estos layers (Player, Ground, etc.).")]
    [SerializeField] LayerMask hitMask;

    Rigidbody2D rb;
    Vector2 dir = Vector2.right;
    bool initialized;

    /// Llamar justo al instanciar para enviar dirección/velocidad
    public void Setup(Vector2 direction, float customSpeed = -1f)
    {
        dir = direction.sqrMagnitude > 0.001f ? direction.normalized : Vector2.right;
        if (customSpeed > 0f) speed = customSpeed;
        ApplyVelocity();
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        dir = transform.right; // <<< ¡ESTA ES LA LÍNEA QUE LO ARREGLA TODO!
    }

    void OnEnable()
    {
        if (lifeTime > 0f) Destroy(gameObject, lifeTime);
        if (!initialized) ApplyVelocity(); // por si no llamaron Setup
    }

    void ApplyVelocity()
    {
        initialized = true;
#if UNITY_6000_0_OR_NEWER
        if (rb) rb.linearVelocity = dir * speed;
#else
        if (rb) rb.velocity = dir * speed;
#endif
        if (faceVelocity && dir.x < 0f)
        {
            var s = transform.localScale;
            s.x = Mathf.Abs(s.x) * -1f;
            transform.localScale = s;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Filtrado por Layer opcional
        if (hitMask.value != 0 && ((1 << other.gameObject.layer) & hitMask) == 0)
            return;

        // 1) Camino moderno: interfaz IDamageable
        var dmg = other.GetComponent<IDamageable>();
        if (dmg != null)
        {
            Vector2 hitPoint = other.ClosestPoint(transform.position);
#if UNITY_6000_0_OR_NEWER
            Vector2 hitDir = rb ? (rb.linearVelocity.sqrMagnitude > 0 ? rb.linearVelocity.normalized : dir) : dir;
#else
            Vector2 hitDir = rb ? (rb.velocity.sqrMagnitude > 0 ? rb.velocity.normalized : dir) : dir;
#endif
            dmg.TakeDamage(damage, hitPoint, hitDir);
            Destroy(gameObject);
            return;
        }

        // 2) Fallback por Tag: Player -> GameManager
        if (other.CompareTag("Player") || other.CompareTag("Player1"))
        {
            GameManager.instancia?.CambiarVidas(-damage);
            Destroy(gameObject);
            return;
        }

        // 3) Si pega con algo sólido, destruir
        if (!other.isTrigger)
            Destroy(gameObject);
    }
}