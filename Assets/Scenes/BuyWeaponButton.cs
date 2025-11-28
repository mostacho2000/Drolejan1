using UnityEngine;
using TMPro;

public class BuyWeaponButton : MonoBehaviour
{
    [Header("Configuración del arma")]
    public WeaponType arma;      // Arma1, Arma2, Arma3
    public int costo = 2000;

    [Header("UI opcional")]
    public TMP_Text textoEtiqueta;     // el texto donde dice "$2000" / "TRATO HECHO"
    public string textoCuandoCompra = "TRATO HECHO";

    public void Comprar()
    {
        var gm = GameManager.instancia;
        if (gm == null)
        {
            Debug.LogError("[BuyWeaponButton] No hay GameManager en escena.");
            return;
        }

        // Intentar comprar
        bool comprado = gm.ComprarArma(arma, costo);
        if (!comprado)
        {
            Debug.Log("[BuyWeaponButton] No alcanzan los puntos para " + arma);
            return;
        }

        Debug.Log("[BuyWeaponButton] Compraste " + arma);

        // Cambiar el texto del cartel
        if (textoEtiqueta != null)
            textoEtiqueta.text = textoCuandoCompra;

        // Si por alguna razón hay un Player en la tienda, actualiza su skin también
        var switcher = FindAnyObjectByType<PlayerSkinSwitcher>();
        if (switcher != null)
        {
            int index = (int)arma + 1; // Arma1=0 -> 1, Arma2=1 -> 2, etc.
            switcher.EquipWeapon(index);
        }
    }
}
