using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerHealth2D : MonoBehaviour, IDamageable
{
    [Header("Vida")]
    [SerializeField] private int maxHealth = 3;
    [SerializeField] private bool destroyOnDeath = false;

    [Header("Invencibilidad")]
    [SerializeField] private float invulnTime = 1.0f;
    [SerializeField] private float blinkInterval = 0.1f;

    [Header("Knockback")]
    [SerializeField] private float hitKnockback = 4f;

    [Header("Animator")]
    [SerializeField] private Animator anim;
    [SerializeField] private string hurtTrigger = "Hurt";
    [SerializeField] private string dieTrigger = "Die";
    [SerializeField] private string deadBool = "Dead";

    [Header("Control de movimiento / disparo")]
    [SerializeField] private PlayerController2D controller;
    [SerializeField] private MonoBehaviour shooter;

    [Header("Game Over")]
    [SerializeField] private string gameOverSceneName = "GameOver Ricardo";
    [SerializeField] private float gameOverDelay = 1.0f;

    [Header("Debug")]
    [SerializeField] private int dbgCurrentHP;

    public int CurrentHP => hp;
    public int MaxHP => maxHealth;

    public Action<int, int> OnHealthChanged;

    int hp;
    bool invuln;
    SpriteRenderer sr;
    Collider2D col;
    Rigidbody2D rb;

    void Awake()
    {
        if (!anim) anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
        if (!controller) controller = GetComponent<PlayerController2D>();

        // 🔹 Si hay GameManager, usamos SUS vidas para arrancar el nivel
        if (GameManager.instancia != null)
        {
            maxHealth = GameManager.instancia.vidasMax;
            hp = Mathf.Clamp(GameManager.instancia.vidas, 0, maxHealth);

            // Si por alguna razón estaba en 0 (venimos de algo raro), lo rellenamos
            if (hp <= 0) hp = maxHealth;
        }
        else
        {
            // Sin GameManager, iniciamos con vida llena
            hp = maxHealth;
        }

        dbgCurrentHP = hp;

        // ⚠️ IMPORTANTE: aquí YA NO llamamos SetVidas(hp).
        // El GameManager ya trae la vida correcta, no la queremos resetear.

        OnHealthChanged?.Invoke(hp, maxHealth);
    }

    // =========================================================
    //  CURAR
    // =========================================================
    public void Heal(int amount)
    {
        hp = Mathf.Clamp(hp + amount, 0, maxHealth);
        dbgCurrentHP = hp;

        if (GameManager.instancia != null)
            GameManager.instancia.SetVidas(hp);

        OnHealthChanged?.Invoke(hp, maxHealth);
    }

    // =========================================================
    //  IDamageable (la bala llama esto)
    // =========================================================
    public void TakeDamage(int amount, Vector2 hitPoint, Vector2 hitDir)
    {
        if (rb)
            rb.AddForce(hitDir.normalized * hitKnockback, ForceMode2D.Impulse);

        Damage(amount);
    }

    public void Damage(int amount)
    {
        if (invuln || hp <= 0) return;

        hp -= amount;
        dbgCurrentHP = hp;

        if (GameManager.instancia != null)
            GameManager.instancia.SetVidas(hp);

        OnHealthChanged?.Invoke(hp, maxHealth);

        if (hp <= 0)
        {
            hp = 0;

            if (anim)
            {
                if (!string.IsNullOrEmpty(deadBool))
                    anim.SetBool(deadBool, true);

                if (!string.IsNullOrEmpty(dieTrigger))
                    anim.SetTrigger(dieTrigger);
            }

            if (controller) controller.enabled = false;
            if (shooter) shooter.enabled = false;
            if (rb) rb.linearVelocity = Vector2.zero;

            StartCoroutine(GameOverCo());
        }
        else
        {
            if (anim && !string.IsNullOrEmpty(hurtTrigger))
                anim.SetTrigger(hurtTrigger);

            StartCoroutine(InvulnCo());
        }
    }

    // =========================================================
    //  INVULNERABILIDAD / PARPADEO
    // =========================================================
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

    // =========================================================
    //  GAME OVER
    // =========================================================
    IEnumerator GameOverCo()
    {
        yield return new WaitForSeconds(gameOverDelay);

        if (GameManager.instancia != null)
        {
            GameManager.instancia.SetVidas(0);
            GameManager.instancia.GameOver();
            yield break;
        }

        if (destroyOnDeath)
            Destroy(gameObject);

        SceneManager.LoadScene(gameOverSceneName);
    }

    // =========================================================
    //  ZONA DE MUERTE INSTANTÁNEA (ACIDO) - CODIGO NUEVO
    // =========================================================

    // Detecta si tocamos un objeto con Collider sólido
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Acido"))
        {
            MuerteInstantanea();
        }
    }

    // Detecta si tocamos un objeto con Collider tipo Trigger (zona)
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Acido"))
        {
            MuerteInstantanea();
        }
    }

    // Función que elimina toda la vida y fuerza la muerte
    public void MuerteInstantanea()
    {
        // Cancelamos invulnerabilidad para asegurar que el daño entre
        invuln = false;

        // Si tenemos vida, aplicamos daño igual a la vida actual para llegar a 0
        if (hp > 0)
        {
            Damage(hp);
            // Al llamar a Damage(), automáticamente se ejecuta la lógica de
            // animaciones, Game Over y desactivación de controles que ya tenías.
        }
    }
}