using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ControladorTienda : MonoBehaviour
{
    [Header("Precios")]
    [SerializeField] private int precioWhisky = 1;
    [SerializeField] private int precioGranada = 1;

    [Header("Refs")]
    [SerializeField] private GameManager controlador;   // lo resolvemos en EnsureGM()

    [Header("UI opcional")]
    [SerializeField] private Button btnWhisky;
    [SerializeField] private Button btnGranada;
    [SerializeField] private TMP_Text txtPrecioWhisky;
    [SerializeField] private TMP_Text txtPrecioGranada;

    private void Awake()
    {
        EnsureGM();
    }

    private void OnEnable()
    {
        EnsureGM();
        // Si el GM re-enlazó el HUD de esta escena, refrescamos para que los botones queden bien
        RefreshButtons();
        // Precios visibles opcionalmente
        if (txtPrecioWhisky)  txtPrecioWhisky.text  = $"${precioWhisky}";
        if (txtPrecioGranada) txtPrecioGranada.text = $"${precioGranada}";
    }

    private void EnsureGM()
    {
        if (!controlador) controlador = GameManager.instancia;
#if UNITY_2023_1_OR_NEWER
        if (!controlador) controlador = Object.FindFirstObjectByType<GameManager>();
#else
        if (!controlador) controlador = FindObjectOfType<GameManager>();
#endif
        if (!controlador)
            Debug.LogError("[TIENDA] No encontré GameManager. ¿Seguro que existe uno persistente?");
        else
            Debug.Log($"[TIENDA] GM ok. Pts={controlador.puntos}  Vida={controlador.vidas}/{controlador.vidasMax}  Gren={controlador.numGranadas}/{controlador.numGranadasMax}");
    }

    private void RefreshButtons()
    {
        if (!controlador) return;
        if (btnWhisky)  btnWhisky.interactable  = controlador.puntos >= precioWhisky  && controlador.vidas       < controlador.vidasMax;
        if (btnGranada) btnGranada.interactable = controlador.puntos >= precioGranada && controlador.numGranadas < controlador.numGranadasMax;
    }

    public void ComprarWhisky()
    {
        EnsureGM();
        Debug.Log($"[TIENDA] Click ComprarWhisky  Pts={controlador?.puntos} Vida={controlador?.vidas}/{controlador?.vidasMax}");
        if (!controlador) return;
        if (controlador.puntos < precioWhisky) { Debug.Log("[TIENDA] No alcanza dinero"); return; }
        if (controlador.vidas >= controlador.vidasMax) { Debug.Log("[TIENDA] Ya en máximo de vidas"); return; }

        controlador.CambiarPuntos(-precioWhisky);
        controlador.CambiarVidas(+1);
        RefreshButtons();
    }

    public void ComprarGranada()
    {
        EnsureGM();
        Debug.Log($"[TIENDA] Click ComprarGranada  Pts={controlador?.puntos} Gren={controlador?.numGranadas}/{controlador?.numGranadasMax}");
        if (!controlador) return;
        if (controlador.puntos < precioGranada) { Debug.Log("[TIENDA] No alcanza dinero"); return; }
        if (controlador.numGranadas >= controlador.numGranadasMax) { Debug.Log("[TIENDA] Ya en máximo de granadas"); return; }

        controlador.CambiarPuntos(-precioGranada);
        controlador.CambiarGranadas(+1);
        RefreshButtons();
    }
}
