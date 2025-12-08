using UnityEngine;

[RequireComponent(typeof(EnemyController2D))]
public class EnemyHealth : MonoBehaviour, IDamageable   // 👈 implementa IDamageable
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

    // === Daño "normal" (balas del player) ===
    public void TakeDamage(int amount)
    {
        if (currentHealth <= 0) return;

        currentHealth -= amount;

        if (currentHealth <= 0)
            Die();
    }

    // === Implementación para granadas (IDamageable) ===
    public void TakeDamage(int amount, Vector2 hitPoint, Vector2 hitDir)
    {
        // reutilizamos la lógica existente
        TakeDamage(amount);
    }

    void Die()
    {
        if (controller != null)
            controller.OnDeath();

        if (anim != null)
            anim.SetTrigger(HASH_DEAD);

        Destroy(gameObject, 0.5f);
    }

    // --- ácido, igual que ya lo tenías ---
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Acido"))
            MuerteInstantanea();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Acido"))
            MuerteInstantanea();
    }

    public void MuerteInstantanea()
    {
        if (currentHealth > 0)
            TakeDamage(currentHealth);
    }
}
