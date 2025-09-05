using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Collider2D))]
public class PlayerGrenade2D : MonoBehaviour
{
    [Header("Input (arrástrame la acción Grenade)")]
    [SerializeField] InputActionReference grenadeRef;   // Button

    [Header("Arrojar")]
    [SerializeField] Transform throwPoint;              // mano/altura del brazo
    [SerializeField] Grenade2D grenadePrefab;
    [SerializeField] float throwForce = 10f;            // fuerza horizontal
    [SerializeField] float upwardForce = 6f;            // arco hacia arriba
    [SerializeField] float cooldown = 0.6f;
    [SerializeField] int grenades = 3;                  // munición (opcional)

    [Header("Visual")]
    [SerializeField] SpriteRenderer sr;                 // para saber si mira a izq/der

    InputAction grenadeAction;
    float cd;
    Collider2D myCol;

    void Awake()
    {
        if (!sr) sr = GetComponent<SpriteRenderer>();
        myCol = GetComponent<Collider2D>();
    }

    void OnEnable()
    {
        grenadeAction = grenadeRef ? grenadeRef.action : null;
        grenadeAction?.Enable();
    }
    void OnDisable() => grenadeAction?.Disable();

    void Update()
    {
        if (grenadeAction == null || grenadePrefab == null || throwPoint == null) return;

        if (cd > 0f) cd -= Time.deltaTime;

        bool pressed = grenadeAction.WasPressedThisFrame();
        if (pressed && cd <= 0f && grenades != 0)
        {
            ThrowGrenade();
            cd = cooldown;
            if (grenades > 0) grenades--;
        }
    }

    void ThrowGrenade()
    {
        Vector2 dir = (sr && sr.flipX) ? Vector2.left : Vector2.right;
        Vector2 vel = dir * throwForce + Vector2.up * upwardForce;

        var g = Instantiate(grenadePrefab, throwPoint.position, Quaternion.identity);
        g.Init(vel, gameObject);

        // Evita colisionar contigo al salir
        var gCol = g.GetComponent<Collider2D>();
        if (myCol && gCol) Physics2D.IgnoreCollision(myCol, gCol, true);
    }
}
