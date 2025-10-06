using UnityEngine;

public class ShopWeaponButton : MonoBehaviour
{
    [Header("Config")]
    public int armaIndex = 1;   // 1 = Arma2, 2 = Arma3, etc.
    public int costo = 2000;

    public void Comprar()
    {
        if (!GameManager.instancia) return;

        if (GameManager.instancia.ComprarArma(armaIndex, costo))
        {
            Debug.Log($"Comprada armaIndex {armaIndex} por {costo}");
            // aquí puedes cambiar el texto del botón a "TRATO HECHO", etc.
        }
        else
        {
            Debug.Log("Puntos insuficientes");
        }
    }
}
