using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Collider2D))]
public class PlayerGrenade2D : MonoBehaviour
{
    [Header("Input (arrastra la acción Grenade)")]
    [SerializeField] private InputActionReference grenadeRef;   // Gameplay/Grenade

    [Header("Arrojar")]
    [SerializeField] private Transform throwPoint;              // pointgranade
    [SerializeField] private Grenade2D grenadePrefab;           // tu prefab "Grenade (Grenade 2D)"
    [SerializeField] private float throwForce = 10f;
    [SerializeField] private float upwardForce = 6f;
    [SerializeField] private float cooldown = 0.6f;

    private InputAction grenadeAction;
    private float nextTime;
    private Collider2D myCol;

    void Awake()
    {
        if (grenadeRef != null)
            grenadeAction = grenadeRef.action;

        myCol = GetComponent<Collider2D>();
    }

    void OnEnable()
    {
        grenadeAction?.Enable();
    }

    void OnDisable()
    {
        grenadeAction?.Disable();
    }

    void Update()
    {
        if (grenadeAction == null)
            return;

        // botón presionado, hay granadas y pasó el cooldown
        if (grenadeAction.WasPressedThisFrame()
            && Time.time >= nextTime
            && (GameManager.instancia == null || GameManager.instancia.TieneGranadas))
        {
            ThrowGrenade();

            // aquí es donde se descuenta y se actualiza la UI
            GameManager.instancia?.CambiarGranadas(-1);

            nextTime = Time.time + cooldown;
        }
    }

    void ThrowGrenade()
    {
        if (grenadePrefab == null || throwPoint == null)
            return;

        // dirección según hacia dónde mira el player (localScale.x)
        float dirX = Mathf.Sign(transform.localScale.x == 0 ? 1f : transform.localScale.x);
        Vector2 dir = new Vector2(dirX, 0f).normalized;

        // instanciar granada
        Grenade2D g = Instantiate(grenadePrefab, throwPoint.position, Quaternion.identity);

        // lanzar con fuerza física
        var grb = g.GetComponent<Rigidbody2D>();
        if (grb != null)
        {
            Vector2 force = dir * throwForce + Vector2.up * upwardForce;
            grb.AddForce(force, ForceMode2D.Impulse);
        }

        // evitar que choque con el propio player al salir
        var gCol = g.GetComponent<Collider2D>();
        if (myCol != null && gCol != null)
            Physics2D.IgnoreCollision(myCol, gCol, true);
    }
}
