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
        anim       = GetComponent<Animator>();
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
            controller.OnDeath();   // ya existe en tu EnemyController2D :contentReference[oaicite:1]{index=1}

        if (anim != null)
            anim.SetTrigger(HASH_DEAD);   // Usa un trigger "Dead" en tu Animator si lo tienes

        // destruir el enemigo después de un ratito
        Destroy(gameObject, 0.5f);
    }
}
