using UnityEngine;

public class SeguirJugadorCamara : MonoBehaviour
{
    [SerializeField] Transform target;                 // arrastra tu Player aquí
    [SerializeField] Vector2 offset = new Vector2(0, 1f);
    [SerializeField] float smoothTime = 0.15f;         // 0 = duro, 0.1–0.3 = suave
    [Header("Límites opcionales (world space)")]
    [SerializeField] bool usarLimites = false;
    [SerializeField] Vector2 minBounds = new Vector2(-100, -10);
    [SerializeField] Vector2 maxBounds = new Vector2( 100,  10);

    Vector3 vel; // usado por SmoothDamp

    void Awake()
    {
        if (!target)
        {
            // Intenta hallar al player por tag si no lo asignaste
            var p = GameObject.FindGameObjectWithTag("Player1") 
                 ?? GameObject.FindGameObjectWithTag("Player");
            if (p) target = p.transform;
        }
    }

    void LateUpdate()
    {
        if (!target) return;

        // Posición deseada + offset, Z fijo en -10
        var desired = new Vector3(target.position.x + offset.x,
                                  target.position.y + offset.y,
                                  -10f);

        // Suavizado
        var pos = Vector3.SmoothDamp(transform.position, desired, ref vel, smoothTime);

        // Límites opcionales
        if (usarLimites)
        {
            pos.x = Mathf.Clamp(pos.x, minBounds.x, maxBounds.x);
            pos.y = Mathf.Clamp(pos.y, minBounds.y, maxBounds.y);
        }

        transform.position = pos;
    }
}
