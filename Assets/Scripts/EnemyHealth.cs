using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider2D))]
public class EnemyHealth : MonoBehaviour, IDamageable
{
    [SerializeField] int maxHP = 3;
    [SerializeField] int pointsOnDeath = 1;
    [SerializeField] float hitKnockback = 4f;

    [Header("Animación")]
    [Tooltip("La duración en segundos de la animación de muerte.")]
    [SerializeField] float tiempoAnimacionMuerte = 1f;

    private Animator animator;
    private int hp;

    // --- NUEVO ---
    // Variable para asegurarnos de que la muerte solo se ejecute una vez
    private bool isDying = false;

    void Awake()
    {
        hp = maxHP;
        animator = GetComponent<Animator>();
    }

    public void TakeDamage(int amount, Vector2 hitPoint, Vector2 hitDir)
    {
        // Si ya está muriendo, no recibir más daño
        if (isDying) return;

        hp -= amount;

        var rb = GetComponent<Rigidbody2D>();
        if (rb) rb.AddForce(hitDir.normalized * hitKnockback, ForceMode2D.Impulse);

        if (hp <= 0)
        {
            // --- MODIFICADO ---
            // Marcar como muriendo y empezar la corutina
            isDying = true;
            GameManager.instancia?.CambiarPuntos(pointsOnDeath);
            StartCoroutine(Die());
        }
    }

    // --- NUEVO MÉTODO ---
    // Se llama automáticamente cuando este objeto entra en un Trigger
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Si ya está muriendo, no hacer nada
        if (isDying) return;

        // Comprobamos si el objeto con el que chocamos tiene el Tag "Acido"
        if (other.CompareTag("Acido"))
        {
            // Marcar como muriendo
            isDying = true;

            // Opcional: Dar puntos también al morir por ácido
            GameManager.instancia?.CambiarPuntos(pointsOnDeath);

            // Iniciar la misma corutina de muerte
            StartCoroutine(Die());
        }
    }

    IEnumerator Die()
    {
        var aiScript = GetComponent<Enemi_shot>();
        if (aiScript) aiScript.enabled = false;

        var rb = GetComponent<Rigidbody2D>();
        if (rb) rb.simulated = false;

        if (animator) animator.SetTrigger("Die");

        yield return new WaitForSeconds(tiempoAnimacionMuerte);

        Destroy(gameObject);
    }
}