using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider2D))]
public class EnemyHealth : MonoBehaviour, IDamageable
{
    [Header("Vida")]
    [SerializeField] int maxHP = 3;
    [SerializeField] int pointsOnDeath = 1;
    [SerializeField] float hitKnockback = 4f;

    [Header("Animación")]
<<<<<<< HEAD
    [Tooltip("Duración de la animación de muerte antes de destruir")]
    [SerializeField] float deathAnimTime = 1f;

    Animator anim;
    Rigidbody2D rb;
    EnemyController2D controller;
    Enemi_shot shooter;
    int hp;
=======
    [Tooltip("La duración en segundos de la animación de muerte.")]
    [SerializeField] float tiempoAnimacionMuerte = 1f;

    private Animator animator;
    private int hp;
>>>>>>> 4e76ed109f553805ee369798c33c53be6dd2a152

    // --- NUEVO ---
    // Variable para asegurarnos de que la muerte solo se ejecute una vez
    private bool isDying = false;

    void Awake()
    {
        hp = maxHP;
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        controller = GetComponent<EnemyController2D>();
        shooter = GetComponent<Enemi_shot>();
    }

    // Firma compatible con tu sistema de daño
    public void TakeDamage(int amount, Vector2 hitPoint, Vector2 hitDir)
    {
        // Si ya está muriendo, no recibir más daño
        if (isDying) return;

        hp -= amount;
        if (rb) rb.AddForce(hitDir.normalized * hitKnockback, ForceMode2D.Impulse);

<<<<<<< HEAD
        if (hp <= 0) StartCoroutine(Die());
    }

=======
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

>>>>>>> 4e76ed109f553805ee369798c33c53be6dd2a152
    IEnumerator Die()
    {
        GameManager.instancia?.CambiarPuntos(pointsOnDeath);

<<<<<<< HEAD
        // desactivar IA y físicas mientras corre la animación
        if (shooter) shooter.enabled = false;
        if (controller) controller.OnDeath();
        if (rb) rb.simulated = false;              // <— mejor que cambiar bodyType o isKinematic

        if (anim) anim.SetTrigger("Die");          // usa tu transición a Muerte
        yield return new WaitForSeconds(deathAnimTime);
=======
        var rb = GetComponent<Rigidbody2D>();
        if (rb) rb.simulated = false;

        if (animator) animator.SetTrigger("Die");

        yield return new WaitForSeconds(tiempoAnimacionMuerte);
>>>>>>> 4e76ed109f553805ee369798c33c53be6dd2a152

        Destroy(gameObject);
    }
}
