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
        // Si toca a un enemigo
        if (other.CompareTag("Enemy"))
        {
            EnemyHealth hp = other.GetComponent<EnemyHealth>();
            if (hp != null)
            {
                hp.TakeDamage(damage);
            }

            Destroy(gameObject); // destruir la bala al impactar
        }
        else if (!other.isTrigger && !other.CompareTag("Player"))
        {
            // Si pega con pared/suelo/otra cosa sólida, también se destruye
            Destroy(gameObject);
        }
    }
}
