using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class TiendaFeriaController : MonoBehaviour
{
    [Header("HUD")]
    public GameObject[] hearts;      // 3 botellas de vida
    public GameObject[] granadas;    // 3 chiles de granada
    public TMP_Text textoPuntos;     // TextMeshPro de "Puntos: X"

    [Header("Precios")]
    public int precioVida = 2;
    public int precioGranada = 1;

    [Header("Botones (opcional)")]
    public Button botonComprarVida;
    public Button botonComprarGranada;

    [Header("Escenas")]
    public string nombreSiguienteNivel = "NivelMuelle";  // cambia por tu escena real

    GameManager gm;

    void Start()
    {
        gm = GameManager.instancia;

        if (gm == null)
        {
            Debug.LogError("[TIENDA] No hay GameManager en escena. Asegúrate de entrar desde el menú.");
            return;
        }

        ActualizarHUD();
        ActualizarInteractables();
    }

    void ActualizarHUD()
    {
        if (gm == null) return;

        // Vidas
        for (int i = 0; i < hearts.Length; i++)
        {
            if (hearts[i] != null)
                hearts[i].SetActive(i < gm.vidas);
        }

        // Granadas
        for (int i = 0; i < granadas.Length; i++)
        {
            if (granadas[i] != null)
                granadas[i].SetActive(i < gm.numGranadas);
        }

        // Puntos
        if (textoPuntos != null)
            textoPuntos.text = "Puntos: " + gm.puntos;
    }

    void ActualizarInteractables()
    {
        if (gm == null) return;

        if (botonComprarVida != null)
            botonComprarVida.interactable = (gm.puntos >= precioVida && gm.vidas < gm.vidasMax);

        if (botonComprarGranada != null)
            botonComprarGranada.interactable = (gm.puntos >= precioGranada && gm.numGranadas < gm.numGranadasMax);
    }

    // ==== Botones ====

    public void ComprarVida()
    {
        if (gm == null) return;

        if (gm.puntos >= precioVida && gm.vidas < gm.vidasMax)
        {
            gm.CambiarPuntos(-precioVida);
            gm.CambiarVidas(+1);
            ActualizarHUD();
            ActualizarInteractables();
        }
    }

    public void ComprarGranada()
    {
        if (gm == null) return;

        if (gm.puntos >= precioGranada && gm.numGranadas < gm.numGranadasMax)
        {
            gm.CambiarPuntos(-precioGranada);
            gm.CambiarGranadas(+1);
            ActualizarHUD();
            ActualizarInteractables();
        }
    }

    public void IrAlSiguienteNivel()
    {
        if (string.IsNullOrEmpty(nombreSiguienteNivel))
        {
            Debug.LogError("[TIENDA] No se ha configurado nombreSiguienteNivel");
            return;
        }

        SceneManager.LoadScene(nombreSiguienteNivel);
    }
}
