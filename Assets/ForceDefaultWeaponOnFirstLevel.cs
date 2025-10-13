using UnityEngine;

public class ForceDefaultWeaponOnFirstLevel : MonoBehaviour
{
    [SerializeField] private WeaponType armaPorDefecto = WeaponType.Arma1;
    [Tooltip("Si lo activas, solo fuerza el arma si no hay nada guardado (primer arranque).")]
    [SerializeField] private bool soloSiNoHayGuardado = false;

    private void Awake()
    {
        if (!GameManager.instancia) return;

        if (soloSiNoHayGuardado)
        {
            // Si ya hay algo guardado, no tocar
            if (PlayerPrefs.HasKey("GM_WEAPON")) return;
        }

        GameManager.instancia.SeleccionarArma(armaPorDefecto);

        // Si tu Player no se auto-actualiza en Start/OnEnable, descomenta:
        // var player = FindObjectOfType<PlayerController2D>();
        // if (player) player.RefreshWeaponFromGameManager(); // método sencillo que aplica el arma actual del GM
    }
}
