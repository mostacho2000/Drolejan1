using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerSkinSwitcher : MonoBehaviour
{
    [Header("Animator Controllers")]
    public AnimatorOverrideController baseController;
    public AnimatorOverrideController arma2Controller;
    public AnimatorOverrideController arma3Controller;

    [Header("Disparo por arma")]
    public GameObject bulletArma1;
    public float fireRateArma1 = 0.5f;

    public GameObject bulletArma2;
    public float fireRateArma2 = 0.45f;

    public GameObject bulletArma3;
    public float fireRateArma3 = 0.40f;

    private Animator anim;
    private PlayerController2D player;

    void Awake()
    {
        anim = GetComponent<Animator>();
        player = GetComponent<PlayerController2D>();
    }

    void Start()
    {
        var arma = GameManager.instancia ? GameManager.instancia.armaSeleccionada : WeaponType.Arma1;
        ApplyWeapon(arma);
    }

    public void ApplyWeapon(WeaponType arma)
    {
        switch (arma)
        {
            default:
            case WeaponType.Arma1:
                if (baseController) anim.runtimeAnimatorController = baseController;
                SetWeaponParams(bulletArma1, fireRateArma1);
                break;

            case WeaponType.Arma2:
                anim.runtimeAnimatorController = arma2Controller ? arma2Controller : baseController;
                SetWeaponParams(bulletArma2, fireRateArma2);
                break;

            case WeaponType.Arma3:
                anim.runtimeAnimatorController = arma3Controller ? arma3Controller : baseController;
                SetWeaponParams(bulletArma3, fireRateArma3);
                break;
        }
    }

    private void SetWeaponParams(GameObject bulletPrefab, float fireRate)
    {
        if (player == null) return;
        player.SetWeaponParams(bulletPrefab, fireRate);
    }
}

