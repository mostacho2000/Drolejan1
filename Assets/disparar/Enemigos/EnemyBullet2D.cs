using UnityEngine;

public class EnemyBullet2D : MonoBehaviour
{
    [Header("Movimiento")]
    public float speed = 6f;
    public Vector2 dir = Vector2.left;

    [Header("Daño")]
    public int damage = 1;

    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void OnEnable()
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = dir.normalized * speed;
    }

    /// Llamar esto al instanciar la bala desde el enemigo
    public void Launch(Vector2 direction, float speedOverride = -1f)
    {
        dir = direction.normalized;
        if (speedOverride > 0) speed = speedOverride;
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = dir * speed;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Ignora otros triggers (por ejemplo, de detección)
        if (other.isTrigger) return;

        // ¿Golpeó al jugador?
        if (other.CompareTag("Player") || other.CompareTag("Player1"))
        {
            // 1) Si tu Player implementa una interfaz de daño:
            var iDamage = other.GetComponent<IDamageable>();
            if (iDamage != null)
            {
                iDamage.ApplyDamage(damage);
            }
            else
            {
                // 2) Si tienes un script PlayerHealth:
                var ph = other.GetComponent<PlayerHealth>();
                if (ph != null) ph.ApplyDamage(damage);
                else
                {
                    // 3) Fallback: usa tu GameManager (ajusta el método al que SÍ tengas)
                    // Si tu GameManager tiene CambiarVidas(int delta):
                    GameManager.instancia?.CambiarVidas(-damage);

                    // Si en tu proyecto el método se llama distinto,
                    // cámbialo por el correcto (p.ej. UpdateHearts(-damage), RestarVidas(damage), etc.)
                }
            }

            Destroy(gameObject);
            return;
        }

        // Si golpea suelo/pared/escenario, destruye la bala
        int ground = LayerMask.NameToLayer("Ground");
        if (other.gameObject.layer == ground)
        {
            Destroy(gameObject);
            return;
        }

        // Evita matar a otros enemigos por accidente
        if (other.GetComponent<EnemyController2D>() != null) return;

        // Por defecto destruye si pega algo sólido
        Destroy(gameObject);
    }

    void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}
