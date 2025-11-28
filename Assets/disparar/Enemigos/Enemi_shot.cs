using UnityEngine;

public class Enemi_shot : MonoBehaviour
{
    [Header("Disparo")]
    public Transform shootOrigin;
    public GameObject bulletPrefab;
    public float fireRate = 1f;      // disparos por segundo
    public float bulletSpeed = 10f;
    public float fireRange = 8f;

    [Header("Línea de vista")]
    public LayerMask obstacleMask;   // paredes / obstáculos. Si está en Nothing, ignora esto.

    float fireTimer = 0f;

    void Awake()
    {
        if (!shootOrigin)
            shootOrigin = transform;
    }

    void Update()
    {
        if (fireTimer > 0f)
            fireTimer -= Time.deltaTime;
    }

    // Comprueba si hay línea de vista libre hacia el target
    public bool HasLOS(Transform target)
    {
        if (!target) return false;

        Vector2 origin = shootOrigin ? shootOrigin.position : transform.position;
        Vector2 dir = (target.position - shootOrigin.position).normalized;
        float dist = Vector2.Distance(origin, target.position);

        if (obstacleMask.value == 0)
        {
            // No hay máscara → asumimos que siempre ve al jugador
            return true;
        }

        RaycastHit2D hit = Physics2D.Raycast(origin, dir, dist, obstacleMask);
        // Si NO golpeamos nada de la máscara, hay línea de vista
        return !hit;
    }

    // Intenta disparar hacia el target
    public void TryShoot(Transform target, Animator anim = null)
    {
        if (!target || !bulletPrefab || fireTimer > 0f)
            return;

        float dist = Vector2.Distance(transform.position, target.position);
        if (dist > fireRange)
            return;

        if (!HasLOS(target))
            return;

        // Reset de cooldown
        fireTimer = 1f / fireRate;

        // Dirección hacia el player
        Vector2 origin = shootOrigin ? shootOrigin.position : transform.position;
        Vector2 dir = (target.position - (Vector3)origin).normalized;

        GameObject bulletGO = Object.Instantiate(bulletPrefab, origin, Quaternion.identity);

        // Asignar velocidad hacia el player
        Rigidbody2D rb = bulletGO.GetComponent<Rigidbody2D>();
        if (rb)
            rb.linearVelocity = dir * bulletSpeed;   // 👉 antes era rb.velocity

        // Rotar la bala para que apunte en la dirección de movimiento (opcional)
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        bulletGO.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        if (anim)
            anim.SetBool("Shoot", true);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, fireRange);
    }
}
