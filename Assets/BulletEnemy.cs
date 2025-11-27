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
        if (collision.CompareTag("Player1"))
        {
            var player = collision.GetComponent<IDamageable>();
            if (player != null)
            {
                Vector2 dir = collision.transform.position - transform.position;
                player.TakeDamage(damage, transform.position, dir);
            }

            Destroy(gameObject);
        }

        // si pega con pisos o paredes
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            Destroy(gameObject);
        }
    }
}
