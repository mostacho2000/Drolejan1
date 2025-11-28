using UnityEngine;

public class PlayerSkinSwitcher : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private PlayerController2D player;           // PlayerController2D del jugador
    [SerializeField] private Animator baseAnimator;              // Animator del player (cuerpo)

    [Header("Animators de Arma")]
    [SerializeField] private RuntimeAnimatorController arma1Controller;
    [SerializeField] private RuntimeAnimatorController arma2Controller;
    [SerializeField] private RuntimeAnimatorController arma3Controller;

    [Header("Balas / Parámetros por arma")]
    [SerializeField] private GameObject bulletArma1;
    [SerializeField] private float fireRateArma1    = 0.5f;
    [SerializeField] private float bulletSpeedArma1 = 18f;

    [SerializeField] private GameObject bulletArma2;
    [SerializeField] private float fireRateArma2    = 0.35f;
    [SerializeField] private float bulletSpeedArma2 = 22f;

    [SerializeField] private GameObject bulletArma3;
    [SerializeField] private float fireRateArma3    = 0.2f;
    [SerializeField] private float bulletSpeedArma3 = 26f;

    [Header("Colliders por arma (opcional)")]
    [SerializeField] private BoxCollider2D playerCollider;
    [SerializeField] private Vector2 offsetArma1   = new Vector2(0f, 0f);
    [SerializeField] private Vector2 sizeArma1     = new Vector2(1f, 2f);

    [SerializeField] private Vector2 offsetArma2   = new Vector2(0f, 0f);
    [SerializeField] private Vector2 sizeArma2     = new Vector2(1f, 2f);

    [SerializeField] private Vector2 offsetArma3   = new Vector2(0f, 0f);
    [SerializeField] private Vector2 sizeArma3     = new Vector2(1f, 2f);

    private int currentWeapon = 1;

    private void Awake()
    {
        if (!player)       player       = GetComponent<PlayerController2D>();
        if (!baseAnimator) baseAnimator = GetComponentInChildren<Animator>();

        // 🔹 Leer del GameManager qué arma está seleccionada
        int index = 1; // por defecto arma 1
        if (GameManager.instancia != null)
        {
            // WeaponType: Arma1 = 0, Arma2 = 1, Arma3 = 2
            index = (int)GameManager.instancia.armaSeleccionada + 1;
        }

        // Equipar el arma correcta al entrar al nivel
        EquipWeapon(index);
    }

    // --------------------------------------------------------------------
    //  MÉTODOS PÚBLICOS PARA LLAMAR DESDE LA TIENDA / BOTONES
    // --------------------------------------------------------------------
    public void EquipWeapon1() => EquipWeapon(1);
    public void EquipWeapon2() => EquipWeapon(2);
    public void EquipWeapon3() => EquipWeapon(3);

    /// <summary>
    /// Equipa el arma según el índice (1, 2 o 3).
    /// </summary>
    public void EquipWeapon(int index)
    {
        currentWeapon = index;

        RuntimeAnimatorController newController = null;
        GameObject newBulletPrefab = null;
        float newFireRate = 0f;
        float newBulletSpeed = 0f;
        Vector2 newOffset = Vector2.zero;
        Vector2 newSize = Vector2.one;

        switch (index)
        {
            case 1:
                newController   = arma1Controller;
                newBulletPrefab = bulletArma1;
                newFireRate     = fireRateArma1;
                newBulletSpeed  = bulletSpeedArma1;
                newOffset       = offsetArma1;
                newSize         = sizeArma1;
                break;

            case 2:
                newController   = arma2Controller;
                newBulletPrefab = bulletArma2;
                newFireRate     = fireRateArma2;
                newBulletSpeed  = bulletSpeedArma2;
                newOffset       = offsetArma2;
                newSize         = sizeArma2;
                break;

            case 3:
                newController   = arma3Controller;
                newBulletPrefab = bulletArma3;
                newFireRate     = fireRateArma3;
                newBulletSpeed  = bulletSpeedArma3;
                newOffset       = offsetArma3;
                newSize         = sizeArma3;
                break;

            default:
                Debug.LogWarning($"[PlayerSkinSwitcher] Índice de arma inválido: {index}");
                return;
        }

        // Cambiar animator del jugador
        if (baseAnimator && newController)
            baseAnimator.runtimeAnimatorController = newController;

        // Pasar parámetros de arma al PlayerController2D
        if (player)
            player.SetWeaponParams(newBulletPrefab, newFireRate, newBulletSpeed);

        // Ajustar collider si está asignado
        if (playerCollider)
        {
            playerCollider.offset = newOffset;
            playerCollider.size   = newSize;
        }

        // 🔹 Guardar selección en el GameManager para siguientes escenas
        if (GameManager.instancia != null)
        {
            WeaponType arma = WeaponType.Arma1;
            if (index == 2) arma = WeaponType.Arma2;
            else if (index == 3) arma = WeaponType.Arma3;

            GameManager.instancia.SeleccionarArma(arma);
        }
    }
}
