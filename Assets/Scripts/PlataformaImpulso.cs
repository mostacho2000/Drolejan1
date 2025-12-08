using UnityEngine;

// Asegúrate de que el nombre del archivo sea el mismo que el de la clase (PlataformaImpulso)
public class PlataformaImpulso : MonoBehaviour
{
    [Tooltip("La fuerza continua que aplicará la plataforma. ¡Sube este número si no se mueve!")]
    [SerializeField]
    private float fuerzaDeEmpuje = 100f; // Súbelo para probar

    private Collider2D platformCollider;

    // Awake se llama una sola vez al inicio
    void Awake()
    {
        platformCollider = GetComponent<Collider2D>();
    }

    // OnCollisionStay2D se llama mientras haya contacto
    private void OnCollisionStay2D(Collision2D collision)
    {
        // 1. Comprueba si es el jugador por su tag
        if (collision.gameObject.CompareTag("Player"))
        {
            // 2. MÉTODO ROBUSTO: Compara las posiciones de los bordes
            Collider2D playerCollider = collision.collider;
            
            // Obtiene el borde SUPERIOR de la plataforma
            float platformTopY = platformCollider.bounds.max.y;
            // Obtiene el borde INFERIOR (los pies) del jugador
            float playerBottomY = playerCollider.bounds.min.y;

            // 3. Si los pies del jugador están sobre la superficie...
            // (Usamos un pequeño margen de error de 0.1f para más estabilidad)
            if (playerBottomY >= platformTopY - 0.1f)
            {
                Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
                if (playerRb != null)
                {
                    // 4. Aplica la fuerza continua hacia la izquierda
                    Vector2 direccionEmpuje = Vector2.left;
                    playerRb.AddForce(direccionEmpuje * fuerzaDeEmpuje);
                }
            }
        }
    }
}