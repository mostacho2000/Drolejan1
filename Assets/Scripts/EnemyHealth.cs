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

    void Awake()
    {
        hp = maxHP;
        animator = GetComponent<Animator>();
    }

    public void TakeDamage(int amount, Vector2 hitPoint, Vector2 hitDir)
    {
        if (hp <= 0) return;

        hp -= amount;

        var rb = GetComponent<Rigidbody2D>();
        if (rb) rb.AddForce(hitDir.normalized * hitKnockback, ForceMode2D.Impulse);

        if (hp <= 0)
        {
            GameManager.instancia?.CambiarPuntos(pointsOnDeath);
            StartCoroutine(Die());
        }
    }
    
    IEnumerator Die()
    {
        var aiScript = GetComponent<Enemi_shot>();
        if (aiScript) aiScript.enabled = false;

        var rb = GetComponent<Rigidbody2D>();
        // --- AQUÍ ESTÁ LA CORRECCIÓN ---
        if (rb) rb.simulated = false; // Se cambia .enabled por .simulated
        
        if (animator) animator.SetTrigger("Die");
        
        yield return new WaitForSeconds(tiempoAnimacionMuerte);

        Destroy(gameObject);
    }
}