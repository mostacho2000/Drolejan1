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
    [SerializeField] float deathAnimTime = 1f;

    Animator anim;
    Rigidbody2D rb;
    EnemyController2D controller;
    Enemi_shot shooter; // Asumiendo que así se llama tu clase

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

    public void TakeDamage(int amount, Vector2 hitPoint, Vector2 hitDir)
    {
        if (isDying) return;

        hp -= amount;

        if (rb)
            rb.AddForce(hitDir.normalized * hitKnockback, ForceMode2D.Impulse);

        if (hp <= 0)
        {
            StartDeathSequence();
        }
    }

    // CASO 1: El acido es un Trigger (Is Trigger marcado)
    public void OnTriggerEnter2D(Collider2D other)
    {
        if (isDying) return;
        if (other.CompareTag("Acido")) StartDeathSequence();
    }

    // CASO 2: El acido es un Collider solido (Suelo, pinchos físicos)
    public void OnCollisionEnter2D(Collision2D other)
    {
        if (isDying) return;
        if (other.gameObject.CompareTag("Acido")) StartDeathSequence();
    }

    // Método unificado para evitar repetir código
    void StartDeathSequence()
    {
        isDying = true;
        StartCoroutine(Die());
    }

    IEnumerator Die()
    {
        GameManager.instancia?.CambiarPuntos(pointsOnDeath);

        if (shooter) shooter.enabled = false;
        if (controller) controller.OnDeath();

        // CAMBIO IMPORTANTE: Detener velocidad en vez de apagar simulación inmediatamente
        if (rb)
        {
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Kinematic; // Se queda quieto pero el Animator funciona
        }

        // Asegúrate que el collider ya no choque con el jugador mientras muere
        GetComponent<Collider2D>().enabled = false;

        if (anim) anim.SetTrigger("Die");

        yield return new WaitForSeconds(deathAnimTime);

        Destroy(gameObject);
    }
}