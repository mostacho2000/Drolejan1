using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class Bullet2D : MonoBehaviour
{
    [Header("Movimiento / Vida")]
    [SerializeField] private float speed = 18f;
    [SerializeField] private float lifeTime = 2f;

    [Header("Daño")]
    [SerializeField] private int damage = 1;
    [SerializeField] private LayerMask hitMask; // Ground, Enemy, etc.

    private Rigidbody2D rb;
    private Vector2 dir = Vector2.right;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void OnEnable()
    {
        Invoke(nameof(Despawn), lifeTime);
    }

    /// <summary> Inicializa dirección y arranca la bala. </summary>
    public void Init(Vector2 direction)
    {
        dir = direction.sqrMagnitude > 0 ? direction.normalized : Vector2.right;
        transform.right = dir;                       // orienta sprite
        rb.linearVelocity = dir * speed;             // velocidad
    }

    void Update()
    {
        // Mantener velocidad estable por si algo la altera
        rb.linearVelocity = dir * speed;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // ¿Golpea algo de la máscara?
        if (((1 << other.gameObject.layer) & hitMask) != 0)
        {
            // Si usas interfaz de daño:
            // other.GetComponent<IDamageable>()?.TakeDamage(damage);
            Despawn();
        }
    }

    private void Despawn()
    {
        CancelInvoke();
        Destroy(gameObject);
        // Futuro: usar pooling en lugar de Destroy()
    }
}
