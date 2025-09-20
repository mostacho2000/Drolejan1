using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class EnemyHealth : MonoBehaviour, IDamageable
{
    [SerializeField] int maxHP = 3;
    [SerializeField] int pointsOnDeath = 1;
    [SerializeField] bool destroyOnDeath = true;
    [SerializeField] float hitKnockback = 4f; // empujón opcional

    int hp;

    void Awake() => hp = maxHP;

    public void TakeDamage(int amount, Vector2 hitPoint, Vector2 hitDir)
    {
        if (hp <= 0) return;

        hp -= amount;

        // pequeño empujón opcional
        var rb = GetComponent<Rigidbody2D>();
        if (rb) rb.AddForce(hitDir.normalized * hitKnockback, ForceMode2D.Impulse);

        if (hp <= 0)
        {
            GameManager.instancia?.CambiarPuntos(pointsOnDeath);
            if (destroyOnDeath) Destroy(gameObject);
        }
    }
}
