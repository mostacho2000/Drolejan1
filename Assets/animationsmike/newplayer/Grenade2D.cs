using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class Grenade2D : MonoBehaviour
{
    [Header("Fusible / Rebotes")]
    [SerializeField] float fuseTime = 2.0f;     // segundos hasta explotar
    [SerializeField] int   maxBounces = 3;      // explota al llegar a este número de rebotes
    [SerializeField] bool  explodeOnEnemyHit = false;

    [Header("Explosión")]
    [SerializeField] float explosionRadius = 2.5f;
    [SerializeField] float explosionForce  = 14f;   // fuerza de empuje a RB2D cercanos
    [SerializeField] int   damage          = 2;
    [SerializeField] LayerMask damageMask;          // Enemigo/Breakable… (NO Player)
    [SerializeField] GameObject explosionFX;        // opcional (partícula/anim)

    [Header("Debug")]
    [SerializeField] bool drawRadius = true;

    Rigidbody2D rb;
    Collider2D col;
    int bounces;
    bool exploded;
    GameObject owner; // para no dañarte a ti mismo

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
    }

    void OnEnable()
    {
        Invoke(nameof(Explode), fuseTime);
    }

    /// Inicializa la granada con velocidad inicial y referencia al dueño
    public void Init(Vector2 initialVelocity, GameObject ownerGO)
    {
        owner = ownerGO;
        rb.linearVelocity = initialVelocity;
        rb.AddTorque(Random.Range(-2f, 2f), ForceMode2D.Impulse);
    }

    void OnCollisionEnter2D(Collision2D c)
    {
        bounces++;

        if (explodeOnEnemyHit &&
            ((1 << c.gameObject.layer) & damageMask) != 0)
        {
            Explode(); return;
        }

        if (bounces >= maxBounces)
            Explode();
    }

    void Explode()
    {
        if (exploded) return;
        exploded = true;
        CancelInvoke();

        // FX visual
        if (explosionFX)
        {
            var fx = Instantiate(explosionFX, transform.position, Quaternion.identity);
            Destroy(fx, 2f);
        }

        // 1) Empuje a todo rigidbody cercano (independiente de capas)
        var hitsAll = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach (var h in hitsAll)
        {
            // omite al dueño
            if (owner && h.transform.root == owner.transform.root) continue;

            var hrb = h.attachedRigidbody;
            if (hrb)
            {
                Vector2 dir = (h.transform.position - transform.position);
                float dist = Mathf.Max(0.1f, dir.magnitude);
                float falloff = Mathf.Clamp01(1f - dist / explosionRadius);
                hrb.AddForce(dir.normalized * explosionForce * falloff, ForceMode2D.Impulse);
            }
        }

        // 2) Daño solo a capas en damageMask
        var hitsDamage = Physics2D.OverlapCircleAll(transform.position, explosionRadius, damageMask);
        foreach (var h in hitsDamage)
        {
            if (owner && h.transform.root == owner.transform.root) continue;

            var dmg = h.GetComponent<IDamageable>();
            if (dmg != null)
            {
                Vector2 dir = ((Vector2)h.transform.position - (Vector2)transform.position).normalized;
                dmg.TakeDamage(damage, transform.position, dir);
            }
        }

        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        if (!drawRadius) return;
        Gizmos.color = new Color(1f, 0.6f, 0.2f, 0.35f);
        Gizmos.DrawSphere(transform.position, explosionRadius);
        Gizmos.color = new Color(1f, 0.6f, 0.2f, 1f);
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
