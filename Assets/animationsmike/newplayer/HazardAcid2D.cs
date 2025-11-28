using UnityEngine;

public class HazardAcid2D : MonoBehaviour
{
    [Header("Daño")]
    public int damage = 1;

    [Header("Filtro")]
    public bool soloEnemigos = true;   // si lo pones en false dañará a cualquiera que implemente IDamageable

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Si solo quieres que dañe enemigos, revisamos el tag
        if (soloEnemigos && !other.CompareTag("Enemy"))
            return;

        // Buscamos algo que implemente IDamageable (como tu EnemyHealth o PlayerHealth2D)
        IDamageable damageable = other.GetComponent<IDamageable>();
        if (damageable != null)
        {
            // Punto y dirección del golpe (no tienen que ser perfectos)
            Vector2 hitPoint = other.ClosestPoint(transform.position);
            Vector2 hitDir   = (other.transform.position - transform.position);

            damageable.TakeDamage(damage, hitPoint, hitDir);
        }
    }
}
