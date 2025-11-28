using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ControladorTienda : MonoBehaviour
{
    [Header("Precios")]
    [SerializeField] private int precioWhisky  = 1;  // +1 vida
    [SerializeField] private int precioGranada = 1;  // +1 granada

    [Header("Refs")]
    [SerializeField] private GameManager controlador;

    [Header("UI opcional")]
    [SerializeField] private Button   btnWhisky;
    [SerializeField] private Button   btnGranada;
    [SerializeField] private TMP_Text txtPrecioWhisky;
    [SerializeField] private TMP_Text txtPrecioGranada;

    void Start()
    {
        EnsureGM();

        // 🔹 Muy importante: enganchar HUD de esta escena
        if (controlador != null)
            controlador.RefrescarHUD();

        RefreshButtons();
    }

    void EnsureGM()
    {
        if (controlador == null)
        {
#if UNITY_2023_1_OR_NEWER
            controlador = FindFirstObjectByType<GameManager>();
#else
            controlador = FindObjectOfType<GameManager>();
#endif
        }

        if (!controlador)
        {
            Debug.LogError("[TIENDA] No encontré GameManager. ¿Seguro que existe uno persistente?");
        }
        else
        {
            Debug.Log($"[TIENDA] GM ok. Pts={controlador.puntos}  Vida={controlador.vidas}/{controlador.vidasMax}  Gren={controlador.numGranadas}/{controlador.numGranadasMax}");
        }
    }

    void RefreshButtons()
    {
        if (!controlador) return;

        if (txtPrecioWhisky)  txtPrecioWhisky.text  = $"${precioWhisky}";
        if (txtPrecioGranada) txtPrecioGranada.text = $"${precioGranada}";

        if (btnWhisky)
        {
            bool puede = controlador.puntos >= precioWhisky &&
                         controlador.vidas < controlador.vidasMax;
            btnWhisky.interactable = puede;
        }

        if (btnGranada)
        {
            bool puede = controlador.puntos >= precioGranada &&
                         controlador.numGranadas < controlador.numGranadasMax;
            btnGranada.interactable = puede;
        }
    }

    public void ComprarWhisky()
    {
        EnsureGM();
        if (!controlador) return;

        Debug.Log($"[TIENDA] Click ComprarWhisky  Pts={controlador.puntos}  Vida={controlador.vidas}/{controlador.vidasMax}");

        if (controlador.puntos < precioWhisky)
        {
            Debug.Log("[TIENDA] No alcanza dinero para whisky");
            return;
        }

        if (controlador.vidas >= controlador.vidasMax)
        {
            Debug.Log("[TIENDA] Ya está en máximo de vidas");
            return;
        }

        controlador.CambiarPuntos(-precioWhisky);
        controlador.CambiarVidas(+1);

        RefreshButtons();
    }

    public void ComprarGranada()
    {
        EnsureGM();
        if (!controlador) return;

        Debug.Log($"[TIENDA] Click ComprarGranada  Pts={controlador.puntos}  Gren={controlador.numGranadas}/{controlador.numGranadasMax}");

        if (controlador.puntos < precioGranada)
        {
            Debug.Log("[TIENDA] No alcanza dinero para granada");
            return;
        }

        if (controlador.numGranadas >= controlador.numGranadasMax)
        {
            Debug.Log("[TIENDA] Ya está en máximo de granadas");
            return;
        }

        controlador.CambiarPuntos(-precioGranada);
        controlador.CambiarGranadas(+1);

        RefreshButtons();
    }
}
