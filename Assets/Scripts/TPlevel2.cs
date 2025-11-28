using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TPlevel2 : MonoBehaviour
{
    [Header("UI (opcional)")]
    public TextMeshProUGUI textoReinciar;

    [Header("Nombres de escenas")]
    [Tooltip("Nombre de la escena del nivel del muelle")]
    public string escenaNivelMuelle = "NivelMuelle";

    [Tooltip("Nombre de la escena del último nivel")]
    public string escenaNivelFinal = "NivelFinal";

    [Tooltip("Escena a la que se irá desde la TIENDA (tiendamike)")]
    public string escenaSiguienteDesdeTienda = "NivelMuelle";

    // --------------------------------------------------------------------
    //  Botón que usas en el TP del muelle (si ya lo tenías)
    // --------------------------------------------------------------------
    public void muelle()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(escenaNivelMuelle);
    }

    // --------------------------------------------------------------------
    //  Botón que usas para ir al último nivel (si ya lo tenías)
    // --------------------------------------------------------------------
    public void finallevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(escenaNivelFinal);
    }

    // --------------------------------------------------------------------
    //  Botón "Siguiente Nivel" de la TIENDA (tiendamike)
    // --------------------------------------------------------------------
    public void SiguienteDesdeTienda()
    {
        Time.timeScale = 1f;

        if (string.IsNullOrEmpty(escenaSiguienteDesdeTienda))
        {
            Debug.LogError("[TPlevel2] No se ha configurado escenaSiguienteDesdeTienda");
            return;
        }

        SceneManager.LoadScene(escenaSiguienteDesdeTienda);
    }
}
