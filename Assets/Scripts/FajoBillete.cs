using UnityEngine;

public class FajoBillete : MonoBehaviour
{
    [Header("Puntos")]
    [SerializeField] int cantidadPuntos = 1;  // edítalo por objeto en el Inspector

    [Header("Feedback (opcional)")]
    [SerializeField] AudioClip sfx;           // sonido al recoger
    [SerializeField] GameObject vfx;          // efecto visual al recoger
    [SerializeField] float destroyDelay = 0.01f;

    bool collected;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (collected) return;
        // acepta Player o Player1 por si tu proyecto mezcla tags
        if (!other.CompareTag("Player") && !other.CompareTag("Player1")) return;

        collected = true;

        // suma puntos al marcador global
        GameManager.instancia?.CambiarPuntos(cantidadPuntos);

        // feedback
        if (sfx) AudioSource.PlayClipAtPoint(sfx, transform.position);
        if (vfx) Instantiate(vfx, transform.position, Quaternion.identity);

        // desactivar visual/colisión inmediatamente para evitar 2 recogidas
        var sr = GetComponent<SpriteRenderer>(); if (sr) sr.enabled = false;
        var col = GetComponent<Collider2D>();   if (col) col.enabled = false;

        Destroy(gameObject, destroyDelay);
    }
}

