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
        if (hp <= 0) return;

        hp -= amount;
        if (rb) rb.AddForce(hitDir.normalized * hitKnockback, ForceMode2D.Impulse);

        if (hp <= 0) StartCoroutine(Die());
    }

    IEnumerator Die()
    {
        GameManager.instancia?.CambiarPuntos(pointsOnDeath);

        // desactivar IA y físicas mientras corre la animación
        if (shooter) shooter.enabled = false;
        if (controller) controller.OnDeath();
        if (rb) rb.simulated = false;              // <— mejor que cambiar bodyType o isKinematic

        if (anim) anim.SetTrigger("Die");          // usa tu transición a Muerte
        yield return new WaitForSeconds(deathAnimTime);

        Destroy(gameObject);
    }
}
