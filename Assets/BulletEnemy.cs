using UnityEngine;

public class BulletEnemy : MonoBehaviour
{
    public int damage = 1;
    public float lifeTime = 4f;
    public float hitKnockback = 4f;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Buscamos el objeto raíz (por si el collider está en un hijo)
        Transform root = collision.transform.root;

        // DEBUG general de impacto
        Debug.Log("Bullet hit: " + collision.name + " (root: " + root.name + ")");

        // 1) ¿Es el player?
        if (root.CompareTag("Player"))
        {
            Debug.Log("✅ La bala tocó al PLAYER");

            // Buscamos IDamageable en el padre o en el mismo objeto
            var damageable = root.GetComponent<IDamageable>();
            if (damageable == null)
                damageable = collision.GetComponent<IDamageable>();

            if (damageable != null)
            {
                Debug.Log("✅ Player tiene IDamageable, aplicando daño");
                Vector2 dir = root.position - transform.position;
                damageable.TakeDamage(damage, transform.position, dir);
            }
            else
            {
                Debug.LogWarning("⚠ La bala tocó al player PERO no encontré IDamageable en él");
            }

            Destroy(gameObject);
            return;
        }

        // 2) Si pega con el piso / pared (layer Ground) se destruye
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            Debug.Log("💥 La bala pegó en el suelo y se destruye");
            Destroy(gameObject);
        }
    }
}
