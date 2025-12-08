using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    [Header("Daño")]
    public int damage = 1;
    public float lifeTime = 3f;

    void Start()
    {
        // destruir la bala si no pega en X segundos
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 1) JEFE FINAL
        JefeFinal jefe = other.GetComponent<JefeFinal>();
        if (jefe != null)
        {
            // punto y dirección del impacto
            Vector2 hitPoint = transform.position;
            Vector2 hitDir   = ((Vector2)other.transform.position - (Vector2)transform.position).normalized;

            jefe.TakeDamage(damage, hitPoint, hitDir);
            Destroy(gameObject);
            return;
        }

        // 2) ENEMIGOS NORMALES (tag "Enemy" + EnemyHealth)
        if (other.CompareTag("Enemy"))
        {
            EnemyHealth hp = other.GetComponent<EnemyHealth>();
            if (hp != null)
            {
                hp.TakeDamage(damage);
            }

            Destroy(gameObject); // destruir la bala al impactar
            return;
        }

        // 3) Si pega con pared/suelo/otra cosa sólida, también se destruye
        if (!other.isTrigger && !other.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }
}
