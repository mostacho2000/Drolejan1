using UnityEngine;

public class EnemyBullet2D : MonoBehaviour
{
    [Header("Daño")]
    public int damage = 1;
    public float lifeTime = 5f;

    private Vector2 direction = Vector2.right;
    private float speed = 5f;

    private Rigidbody2D rb;

    // Lo llama el jefe cuando instancia la bala
    public void Setup(Vector2 dir, float bulletSpeed)
    {
        direction = dir.normalized;
        speed = bulletSpeed;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        Destroy(gameObject, lifeTime); // Destruye la bala si no pega
    }

    private void FixedUpdate()
    {
        if (rb != null)
        {
#if UNITY_6000_0_OR_NEWER
            rb.linearVelocity = direction * speed;
#else
            rb.velocity = direction * speed;
#endif
        }
        else
        {
            transform.position += (Vector3)(direction * speed * Time.fixedDeltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Solo dañamos al player
        if (other.CompareTag("Player") || other.CompareTag("Player1"))
        {
            var dmg = other.GetComponent<IDamageable>();
            if (dmg != null)
            {
                Vector2 hitPoint = transform.position;
                Vector2 hitDir = direction;

                dmg.TakeDamage(damage, hitPoint, hitDir);
            }

            Destroy(gameObject);
            return;
        }

        // Si pega con piso, pared, etc. (collider sólido)
        if (!other.isTrigger)
        {
            Destroy(gameObject);
        }
    }
}
