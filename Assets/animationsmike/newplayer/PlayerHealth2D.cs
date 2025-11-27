using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider2D))]
public class PlayerHealth2D : MonoBehaviour, IDamageable
{
    [Header("Vida")]
    [SerializeField] private int maxHealth = 5;
    [SerializeField] private bool destroyOnDeath = false;

    [Header("Invencibilidad")]
    [SerializeField] private float invulnTime = 1.0f;
    [SerializeField] private float blinkInterval = 0.1f;

    [Header("Knockback")]
    [SerializeField] private float hitKnockback = 4f;

    [Header("Animator (coincidir con tu controller)")]
    [SerializeField] private Animator anim;
    [SerializeField] private string hurtTrigger = "Hurt";
    [SerializeField] private string dieTrigger = "Die";
    [SerializeField] private string deadBool = "Dead";

    [Header("Control de movimiento/disparo")]
    [SerializeField] private PlayerController2D controller;
    [SerializeField] private MonoBehaviour shooter;

    [Header("Game Over")]
    [SerializeField] private string gameOverSceneName = "GameOver Ricardo";
    [SerializeField] private float gameOverDelay = 1.0f;

    [Header("Debug")]
    [SerializeField] private int dbgCurrentHP;

    // Propiedades para la UI
    public int CurrentHP => hp;
    public int MaxHP    => maxHealth;

    // Evento para que la UI se actualice
    public Action<int, int> OnHealthChanged;

    int hp;
    bool invuln;
    SpriteRenderer sr;
    Collider2D col;
    Rigidbody2D rb;

    void Awake()
    {
        hp = maxHealth;
        dbgCurrentHP = hp;

        if (!anim)      anim = GetComponent<Animator>();
        sr  = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
        rb  = GetComponent<Rigidbody2D>();
        if (!controller) controller = GetComponent<PlayerController2D>();

        // avisar al Canvas del valor inicial de vida
        OnHealthChanged?.Invoke(hp, maxHealth);
    }

    // Curar vida (por si luego usas botellas, etc.)
    public void Heal(int amount)
    {
        hp = Mathf.Clamp(hp + amount, 0, maxHealth);
        dbgCurrentHP = hp;

        OnHealthChanged?.Invoke(hp, maxHealth);
    }

    // IMPLEMENTACIÓN DE IDAMAGEABLE (la bala llama esto)
    public void TakeDamage(int amount, Vector2 hitPoint, Vector2 hitDir)
    {
        if (rb)
            rb.AddForce(hitDir.normalized * hitKnockback, ForceMode2D.Impulse);

        Damage(amount);
    }

    // Lógica principal de daño
    public void Damage(int amount)
    {
        if (invuln || hp <= 0) return;

        hp -= amount;
        dbgCurrentHP = hp;

        // avisar al Canvas
        OnHealthChanged?.Invoke(hp, maxHealth);

        if (hp <= 0)
        {
            hp = 0;

            // Animación de muerte
            if (anim)
            {
                if (!string.IsNullOrEmpty(deadBool))
                    anim.SetBool(deadBool, true);

                if (!string.IsNullOrEmpty(dieTrigger))
                    anim.SetTrigger(dieTrigger);
            }

            // apagar controles
            if (controller) controller.enabled = false;
            if (shooter)    shooter.enabled    = false;

            if (rb) rb.linearVelocity = Vector2.zero;

            // lanzar corrutina de Game Over
            StartCoroutine(GameOverCo());
        }
        else
        {
            // Anim de daño + invencibilidad con parpadeo
            if (anim && !string.IsNullOrEmpty(hurtTrigger))
                anim.SetTrigger(hurtTrigger);

            StartCoroutine(InvulnCo());
        }
    }

    IEnumerator InvulnCo()
    {
        invuln = true;
        float t = 0f;
        bool visible = true;

        while (t < invulnTime)
        {
            t += blinkInterval;
            visible = !visible;

            if (sr) sr.enabled = visible;

            yield return new WaitForSeconds(blinkInterval);
        }

        if (sr) sr.enabled = true;
        invuln = false;
    }

    IEnumerator GameOverCo()
    {
        // espera a que se vea la anim de muerte
        yield return new WaitForSeconds(gameOverDelay);

        // opcional: destruir el player antes de cambiar de escena
        if (destroyOnDeath)
            Destroy(gameObject);

        SceneManager.LoadScene(gameOverSceneName);
    }
}
