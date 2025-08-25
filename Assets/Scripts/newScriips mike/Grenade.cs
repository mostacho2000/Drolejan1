using System.Collections;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    [Header("Explosión")]
    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] private float fuseSeconds = 2.0f;      // explota sola después de este tiempo
    [SerializeField] private bool explodeOnImpact = true;    // explotar al primer choque
    [SerializeField] private int maxBounces = 0;             // si no explotas por impacto: rebotes antes de explotar
    [SerializeField] private float destroyDelay = 0.02f;     // margen para destruir tras instanciar fx

    [Header("Impacto (capas que SÍ activan la explosión)")]
    [Tooltip("Si es 0, explota con cualquier cosa. Si no, solo con estas capas.")]
    [SerializeField] private LayerMask explodeOnLayers = ~0; // por defecto: todas

    [Header("Daño en área")]
    [SerializeField] private float explosionRadius = 2.5f;
    [SerializeField] private float knockbackForce = 8f;
    [SerializeField] private LayerMask damageMask;           // capas afectadas por el daño/impulso

    [Header("Opcional")]
    [SerializeField] private AudioSource audioSource;        // si quieres sonido
    [SerializeField] private AudioClip explosionSfx;

    private bool exploded = false;
    private int bounceCount = 0;

    // (Opcional) para no dañar al lanzador en el primer frame
    private Collider2D ownerCollider;
    public void SetOwnerCollider(Collider2D col) => ownerCollider = col;

    private void OnEnable()
    {
        // Ignora colisión con el owner el primer frame (si se setea)
        if (ownerCollider && TryGetComponent<Collider2D>(out var mine))
            Physics2D.IgnoreCollision(mine, ownerCollider, true);

        StartCoroutine(FuseRoutine());
    }

    private IEnumerator FuseRoutine()
    {
        if (fuseSeconds > 0f)
            yield return new WaitForSeconds(fuseSeconds);

        Explode();
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        // Re‑habilitar colisión con owner a partir del primer impacto
        if (ownerCollider && TryGetComponent<Collider2D>(out var mine))
            Physics2D.IgnoreCollision(mine, ownerCollider, false);

        bounceCount++;

        // Si tengo un filtro por capas, y la capa NO está incluida → no exploto (solo reboté)
        bool layerMatches = (explodeOnLayers == 0) || ((explodeOnLayers.value & (1 << col.collider.gameObject.layer)) != 0);

        if (explodeOnImpact && layerMatches)
        {
            Explode();
            return;
        }

        if (!explodeOnImpact && bounceCount > maxBounces)
        {
            Explode();
        }
    }

    private void Explode()
    {
        if (exploded) return;
        exploded = true;

        // Sfx
        if (audioSource && explosionSfx)
            audioSource.PlayOneShot(explosionSfx);

        // FX visual
        if (explosionPrefab)
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);

        // Daño / knockback por radio
        if (explosionRadius > 0.01f)
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, explosionRadius, damageMask);
            foreach (var h in hits)
            {
                // Knockback si tiene Rigidbody2D
                if (h.attachedRigidbody)
                {
                    Vector2 dir = (h.attachedRigidbody.worldCenterOfMass - (Vector2)transform.position).normalized;
                    h.attachedRigidbody.AddForce(dir * knockbackForce, ForceMode2D.Impulse);
                }

                // Notificar daño si implementa la interfaz
                var dmg = h.GetComponent<IDamageable>();
                if (dmg != null)
                    dmg.TakeDamage(1);
                else
                    h.SendMessage("TakeDamage", 1, SendMessageOptions.DontRequireReceiver); // fallback opcional
            }
        }

        // Destruir la granada (si hay audio, deja un pelín de delay)
        Destroy(gameObject, destroyDelay);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0.5f, 0f, 0.25f);
        Gizmos.DrawSphere(transform.position, explosionRadius);
        Gizmos.color = new Color(1f, 0.5f, 0f, 1f);
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}

// Contrato opcional para objetos que reciben daño.
// Implementa esto en tu enemigo o breakable.
public interface IDamageable
{
    void TakeDamage(int amount);
}
