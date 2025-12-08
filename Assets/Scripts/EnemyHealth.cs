using UnityEngine;

[RequireComponent(typeof(EnemyController2D))]
public class EnemyHealth : MonoBehaviour
{
    [Header("Vida")]
    public int maxHealth = 3;
    int currentHealth;

    EnemyController2D controller;
    Animator anim;

    static readonly int HASH_DEAD = Animator.StringToHash("Dead");

    void Awake()
    {
        currentHealth = maxHealth;
        controller = GetComponent<EnemyController2D>();
        anim = GetComponent<Animator>();
    }

    public void TakeDamage(int amount)
    {
        if (currentHealth <= 0) return;

        currentHealth -= amount;

        // Aquí puedes meter anim de daño si quieres

        if (currentHealth <= 0)
            Die();
    }

    void Die()
    {
        if (controller != null)
            controller.OnDeath();   // ya existe en tu EnemyController2D

        if (anim != null)
            anim.SetTrigger(HASH_DEAD);   // Usa un trigger "Dead" en tu Animator si lo tienes

        // destruir el enemigo después de un ratito
        Destroy(gameObject, 0.5f);
    }

    // =========================================================
    //  NUEVA FUNCIONALIDAD: ACIDO / MUERTE INSTANTANEA
    // =========================================================

    // 1. Detectar colisión física (si el ácido es sólido para el enemigo)
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Acido"))
        {
            MuerteInstantanea();
        }
    }

    // 2. Detectar trigger (si el ácido es una zona o líquido)
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Acido"))
        {
            MuerteInstantanea();
        }
    }

    // 3. Función para matar instantáneamente al enemigo
    public void MuerteInstantanea()
    {
        if (currentHealth > 0)
        {
            // Le aplicamos daño igual a su vida actual para llevarlo a 0.
            // Esto reutiliza tu función TakeDamage() y asegura que se llame a Die().
            TakeDamage(currentHealth);
        }
    }
}