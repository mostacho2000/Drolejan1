using UnityEngine;

public class ShopWeaponButton : MonoBehaviour
{
    [Header("Config")]
    public WeaponType arma = WeaponType.Arma2;   // selecciona qué arma vende este botón
    public int costo = 2000;

    public void Comprar()
    {
        if (GameManager.instancia == null)
            return;

        if (GameManager.instancia.ComprarArma(arma, costo))
        {
            Debug.Log($"Comprada arma {arma} por {costo}");
            // aquí puedes cambiar el texto del botón a "TRATO HECHO", desactivar el botón, etc.
        }
        else
        {
            Debug.Log("Puntos insuficientes");
        }
    }
}
