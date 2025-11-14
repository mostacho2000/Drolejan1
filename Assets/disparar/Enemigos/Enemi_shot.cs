using UnityEngine;

public class Enemi_shot : MonoBehaviour
{
    [Header("Disparo")]
    [Tooltip("Punto desde donde sale la bala")]
    public Transform shootOrigin;
    public GameObject bulletPrefab;

    [Tooltip("Balas por segundo (1 / cooldown)")]
    public float fireRate = 1f;
    [Tooltip("Velocidad de la bala")]
    public float bulletSpeed = 12f;
    [Tooltip("Alcance máx. para permitir el disparo")]
    public float fireRange = 9f;

    [Header("Línea de vista")]
    [Tooltip("Capas que bloquean la visión: Ground, Platforms, Walls…")]
    public LayerMask obstacleMask;

    float _nextFire;

    public bool InRange(Transform target)
    {
        if (!target) return false;
        return Vector2.Distance(transform.position, target.position) <= fireRange && HasLOS(target);
    }

    public void TryShoot(Transform target, Animator anim = null)
    {
        if (!target) return;
        if (Time.time < _nextFire) return;
        if (!InRange(target)) return;

        _nextFire = Time.time + 1f / Mathf.Max(0.01f, fireRate);

        if (anim) anim.SetBool("Shoot", true);

        Vector2 dir = (target.position - shootOrigin.position).normalized;
        var bullet = Instantiate(bulletPrefab, shootOrigin.position, Quaternion.identity);
        var rb = bullet.GetComponent<Rigidbody2D>();
        if (rb) rb.linearVelocity = dir * bulletSpeed;

        // Evitar golpearse a sí mismo
        var myCol = GetComponent<Collider2D>();
        var bCol = bullet.GetComponent<Collider2D>();
        if (myCol && bCol) Physics2D.IgnoreCollision(bCol, myCol);
    }

    public bool HasLOS(Transform target)
    {
        Vector2 toTarget = (target.position - shootOrigin.position);
        var hit = Physics2D.Raycast(shootOrigin.position, toTarget.normalized, toTarget.magnitude, obstacleMask);
        return hit.collider == null;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red; Gizmos.DrawWireSphere(transform.position, fireRange);
        if (shootOrigin) { Gizmos.color = Color.yellow; Gizmos.DrawLine(shootOrigin.position, shootOrigin.position + Vector3.right * 0.3f); }
    }
}
