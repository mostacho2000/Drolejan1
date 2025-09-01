using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement; // opcional si luego quieres recargar escena

[RequireComponent(typeof(Collider2D))]
public class PlayerHealth2D : MonoBehaviour
{
    [Header("Vida")]
    [SerializeField] private int maxHealth = 5;
    [SerializeField] private bool destroyOnDeath = false; // si no, solo bloquea controles

    [Header("Invencibilidad al ser golpeado")]
    [SerializeField] private float invulnTime = 1.0f;
    [SerializeField] private float blinkInterval = 0.1f;

    [Header("Animator (coincidir con tu controller)")]
    [SerializeField] private Animator anim;        // si no lo asignas, lo busco en Awake
    [SerializeField] private string hurtTrigger = "Hurt"; // usa tu daño_strip8
    [SerializeField] private string dieTrigger  = "Die";  // usa tu muerte_strip25
    [SerializeField] private string deadBool    = "Dead"; // opcional

    [Header("Control de movimiento/disparo")]
    [SerializeField] private PlayerController2D controller; // tu script de movimiento
    [SerializeField] private MonoBehaviour shooter;         // si disparas en script aparte (PlayerFire2D)

    // Debug (opcional en el Inspector)
    [SerializeField] private int dbgCurrentHP;

    int hp;
    bool invuln;
    SpriteRenderer sr;
    Collider2D col;
    Rigidbody2D rb;

    void Awake()
    {
        hp = maxHealth;
        dbgCurrentHP = hp;

        if (!anim) anim = GetComponent<Animator>();
        sr  = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
        rb  = GetComponent<Rigidbody2D>();
        if (!controller) controller = GetComponent<PlayerController2D>();
    }

    public void Heal(int amount)
    {
        hp = Mathf.Clamp(hp + amount, 0, maxHealth);
        dbgCurrentHP = hp;
    }

    public void Damage(int amount)
    {
        if (invuln || hp <= 0) return;

        hp -= amount;
        dbgCurrentHP = hp;

        if (hp <= 0)
        {
            hp = 0;
            // Anim de muerte
            if (anim)
            {
                if (!string.IsNullOrEmpty(deadBool)) anim.SetBool(deadBool, true);
                if (!string.IsNullOrEmpty(dieTrigger)) anim.SetTrigger(dieTrigger);
            }
            // Bloquea controles
            if (controller) controller.enabled = false;
            if (shooter) shooter.enabled = false;

            // Opcional: congela movimiento
            if (rb) rb.linearVelocity = Vector2.zero;

            if (destroyOnDeath)
                StartCoroutine(DieDestroyCo());
            // Si no destruyes, te quedas “muerto” hasta que hagas respawn manual
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
        bool v = true;
        while (t < invulnTime)
        {
            t += blinkInterval;
            v = !v;
            if (sr) sr.enabled = v;
            yield return new WaitForSeconds(blinkInterval);
        }
        if (sr) sr.enabled = true;
        invuln = false;
    }

    IEnumerator DieDestroyCo()
    {
        // Espera a que termine el clip de muerte (ajusta si tu clip dura más/menos)
        yield return new WaitForSeconds(1.0f);
        Destroy(gameObject);
        // O, si prefieres, recarga la escena:
        // SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // Útil si quieres probar desde un trigger o botón
    [ContextMenu("Test: Damage 1")]
    void TestDamage() => Damage(1);
}
