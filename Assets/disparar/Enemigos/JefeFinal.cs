using UnityEngine;
using UnityEngine.SceneManagement;

public class JefeFinal : MonoBehaviour, IDamageable
{
    [Header("Vida")]
    [SerializeField] int maxHP = 5;
    [SerializeField] int puntosAlMorir = 3;   // opcional, suma puntos al morir
    int hp;

    [Header("Disparo del jefe")]
    [SerializeField] Transform shootOrigin;   // antes "Insatncia"
    [SerializeField] GameObject bulletPrefab; // antes "bala"
    [SerializeField] float shootInterval = 2f;
    float timer;

    [Header("Escena al morir")]
    [SerializeField] string winScene = "Escena Win Ricardo";

    void Awake()
    {
        hp = Mathf.Max(1, maxHP);
    }

    void Update()
    {
        // Disparo automático cada shootInterval segundos
        timer += Time.deltaTime;
        if (timer >= shootInterval)
        {
            if (bulletPrefab && shootOrigin)
            {
                Instantiate(bulletPrefab, shootOrigin.position, Quaternion.identity);
            }
            timer = 0f;
        }
    }

    // ==== Compatibilidad con tu sistema de balas (Bullet2D -> IDamageable) ====
    public void TakeDamage(int amount, Vector2 hitPoint, Vector2 hitDir)
    {
        if (hp <= 0) return;
        hp -= Mathf.Max(1, amount);
        if (hp <= 0) Die();
    }

    // ==== Compatibilidad con tus tags antiguos (balaBuena / granada) ====
    // Recuerda: esto se ejecuta porque las balas/plantas usan collider con IsTrigger = true.
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hp <= 0) return;

        // 1) Bala del player por tag antiguo
        if (other.CompareTag("balaBuena"))
        {
            hp -= 1;
            // destruye la bala si así lo quieres
            Destroy(other.gameObject);
            if (hp <= 0) Die();
            return;
        }

        // 2) Granada por tag antiguo
        if (other.CompareTag("granada"))
        {
            hp -= 3; // o el daño que quieras
            if (hp <= 0) Die();
        }
    }

    void Die()
    {
        // Puntos (opcional)
        GameManager.instancia?.CambiarPuntos(puntosAlMorir);

        // Destruir jefe y cargar escena de victoria
        Destroy(gameObject);
        if (!string.IsNullOrEmpty(winScene))
            SceneManager.LoadScene(winScene);
    }
}
