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
    private float nextTime;                                     // control de cooldown
    private SpriteRenderer sr;
    private Collider2D myCol;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        myCol = GetComponent<Collider2D>();
    }

    void OnEnable()
    {
        grenadeAction = grenadeRef ? grenadeRef.action : null;
        grenadeAction?.Enable();
    }

    void OnDisable()
    {
        grenadeAction?.Disable();
    }

    void Update()
    {
        if (grenadeAction == null) return;

        // botón presionado, hay granadas y pasó el cooldown
        if (grenadeAction.WasPressedThisFrame()
            && Time.time >= nextTime
            && (GameManager.instancia == null || GameManager.instancia.TieneGranadas()))
        {
            ThrowGrenade();
            // ⬇️ AQUÍ ES DONDE SE DESCUENTA Y SE ACTUALIZA LA UI
            GameManager.instancia?.CambiarGranadas(-1);

            nextTime = Time.time + cooldown;
        }
    }

    private void ThrowGrenade()
    {
        if (!grenadePrefab || !throwPoint) return;

        // dirección según flip del sprite
        Vector2 dir = (sr && sr.flipX) ? Vector2.left : Vector2.right;

        // instancia
        var g = Instantiate(grenadePrefab, throwPoint.position, Quaternion.identity);

        // lanzar (si tu Grenade2D tiene método Launch, úsalo; si no, fuerza directa)
        var grb = g.GetComponent<Rigidbody2D>();
        if (grb)
        {
            Vector2 force = dir * throwForce + Vector2.up * upwardForce;
            grb.AddForce(force, ForceMode2D.Impulse);
        }

        // evitar que choque con el propio player al salir
        var gCol = g.GetComponent<Collider2D>();
        if (myCol && gCol) Physics2D.IgnoreCollision(myCol, gCol, true);
    }
}
