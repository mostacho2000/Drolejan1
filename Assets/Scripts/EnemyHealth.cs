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
    [Tooltip("Duración de la animación de muerte antes de destruir")]
    [SerializeField] float deathAnimTime = 1f;

    Animator anim;
    Rigidbody2D rb;
    EnemyController2D controller;
    Enemi_shot shooter;

    int hp;
    bool isDying = false;

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
        if (isDying) return;

        hp -= amount;

        if (rb)
            rb.AddForce(hitDir.normalized * hitKnockback, ForceMode2D.Impulse);

        if (hp <= 0)
        {
            isDying = true;
            StartCoroutine(Die());
        }
    }

    // Muerte por áreas/tiles con tag "Acido"
    public void OnTriggerEnter2D(Collider2D other)
    {
        if (isDying) return;

        if (other.CompareTag("Acido"))
        {
            isDying = true;
            StartCoroutine(Die());
        }
    }

    IEnumerator Die()
    {
        // Da puntos UNA sola vez
        GameManager.instancia?.CambiarPuntos(pointsOnDeath);

        // Desactiva IA y físicas para que la anim corra limpia
        if (shooter) shooter.enabled = false;
        if (controller) controller.OnDeath();  // si no tienes este método, quita esta línea
        if (rb) rb.simulated = false;

        if (anim) anim.SetTrigger("Die");      // asegúrate de tener el trigger "Die" en el Animator
        yield return new WaitForSeconds(deathAnimTime);

        Destroy(gameObject);
    }
}
