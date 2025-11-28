using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public enum WeaponType { Arma1 = 0, Arma2 = 1, Arma3 = 2 }

public class GameManager : MonoBehaviour
{
    public static GameManager instancia;

    [Header("UI (auto-bind por nombre: Puntaje / Hearts / Granadas)")]
    [SerializeField] private TextMeshProUGUI textMesh;   // Objeto "Puntaje"
    [SerializeField] private GameObject[] hearts;        // Hijos de "Hearts"
    [SerializeField] private GameObject[] granadas;      // Hijos de "Granadas"

    [Header("Valores iniciales")]
    public int vidasMax = 3;
    public int vidas    = 3;

    public int numGranadasMax = 3;
    public int numGranadas    = 3;

    public int puntos = 0;

    [Header("Arma seleccionada (persiste entre escenas)")]
    public WeaponType armaSeleccionada = WeaponType.Arma1;

    [Header("Persistencia / Escenas")]
    [SerializeField] private bool dontDestroyOnLoad = true;
    [SerializeField] private string gameOverSceneName = "GameOver Ricardo";

    // --- PlayerPrefs keys ---
    const string KEY_WEAPON    = "GM_WEAPON";
    const string KEY_VIDAS     = "GM_VIDAS";
    const string KEY_GRANADAS  = "GM_GRANADAS";
    const string KEY_PUNTOS    = "GM_PUNTOS";

    // =========================================================

    void Awake()
    {
        if (instancia != null && instancia != this)
        {
            Destroy(gameObject);
            return;
        }

        instancia = this;
        if (dontDestroyOnLoad)
            DontDestroyOnLoad(gameObject);

        // Cargar arma seleccionada
        armaSeleccionada = (WeaponType)PlayerPrefs.GetInt(KEY_WEAPON, 0);

        // Cargar estado guardado si existe
        if (PlayerPrefs.HasKey(KEY_VIDAS))
        {
            vidas       = PlayerPrefs.GetInt(KEY_VIDAS, vidasMax);
            numGranadas = PlayerPrefs.GetInt(KEY_GRANADAS, numGranadasMax);
            puntos      = PlayerPrefs.GetInt(KEY_PUNTOS, 0);
        }

        TryAutoBindUI();
        ClampAll();
        RefreshAll();
    }

    void OnEnable()  => SceneManager.sceneLoaded += OnSceneLoaded;
    void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

    void OnSceneLoaded(Scene s, LoadSceneMode mode)
    {
        // Cuando cambia de escena, volvemos a enlazar la HUD
        RefrescarHUD();
    }

    // =========================================================
    //  GUARDAR / CARGAR ESTADO BÁSICO
    // =========================================================

    void SaveState()
    {
        PlayerPrefs.SetInt(KEY_VIDAS, vidas);
        PlayerPrefs.SetInt(KEY_GRANADAS, numGranadas);
        PlayerPrefs.SetInt(KEY_PUNTOS, puntos);
        PlayerPrefs.Save();
    }

    [ContextMenu("Resetear vidas y granadas al máximo")]
    public void ResetVidasYGranadas()
    {
        vidas       = vidasMax;
        numGranadas = numGranadasMax;
        ClampAll();
        RefreshAll();
        SaveState();
    }

    // ========= NUEVA PARTIDA (desde el menú) =========
    public void NuevoJuego()
    {
        vidas       = vidasMax;
        numGranadas = numGranadasMax;
        puntos      = 0;
        armaSeleccionada = WeaponType.Arma1;

        ClampAll();
        RefreshAll();

        PlayerPrefs.SetInt(KEY_WEAPON, (int)armaSeleccionada);
        SaveState();
    }

    // ========= Refrescar HUD desde fuera (tienda, etc.) =========
    public void RefrescarHUD()
    {
        TryAutoBindUI();   // busca Hearts / Granadas / Puntaje de ESTA escena
        ClampAll();
        RefreshAll();      // actualiza íconos según vidas / granadas actuales
    }

    // =========================================================
    //  API DE VIDAS / GRANADAS / PUNTOS
    // =========================================================

    public void CambiarVidas(int delta)
    {
        vidas = Mathf.Clamp(vidas + delta, 0, vidasMax);
        UpdateHearts();
        SaveState();

        if (vidas <= 0)
            GameOver();
    }

    public void SetVidas(int value)
    {
        vidas = Mathf.Clamp(value, 0, vidasMax);
        UpdateHearts();
        SaveState();
    }

    public void CambiarGranadas(int delta)
    {
        numGranadas = Mathf.Clamp(numGranadas + delta, 0, numGranadasMax);
        UpdateGranadas();
        SaveState();
    }

    public void SetGranadas(int value)
    {
        numGranadas = Mathf.Clamp(value, 0, numGranadasMax);
        UpdateGranadas();
        SaveState();
    }

    public bool TieneGranadas => numGranadas > 0;

    public void CambiarPuntos(int delta)
    {
        puntos += delta;
        UpdatePuntos();
        SaveState();
    }

    public void AddScore(int amount) => CambiarPuntos(amount);

    // =========================================================
    //  ARMAS / TIENDA
    // =========================================================

    public bool PuedeComprar(int costo) => puntos >= costo;

    public bool ComprarArma(WeaponType arma, int costo)
    {
        if (!PuedeComprar(costo))
            return false;

        puntos -= costo;
        UpdatePuntos();

        SeleccionarArma(arma);

        PlayerPrefs.SetInt($"OWN_WEAPON_{(int)arma}", 1);
        PlayerPrefs.Save();

        SaveState();
        return true;
    }

    public bool TieneArma(WeaponType arma)
    {
        return PlayerPrefs.GetInt($"OWN_WEAPON_{(int)arma}", 0) == 1;
    }

    public void SeleccionarArma(WeaponType arma)
    {
        armaSeleccionada = arma;
        PlayerPrefs.SetInt(KEY_WEAPON, (int)armaSeleccionada);
        PlayerPrefs.Save();
    }

    // =========================================================
    //  GAME OVER
    // =========================================================

    public void GameOver()
    {
        Debug.Log("[GameManager] GAME OVER");
        if (!string.IsNullOrEmpty(gameOverSceneName))
            SceneManager.LoadScene(gameOverSceneName);
        else
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void GameOverr() => GameOver();

    // =========================================================
    //  UI INTERNA
    // =========================================================

    void UpdateHearts()
    {
        if (hearts == null) return;
        for (int i = 0; i < hearts.Length; i++)
        {
            if (hearts[i] != null)
                hearts[i].SetActive(i < vidas);
        }
    }

    void UpdateGranadas()
    {
        if (granadas == null) return;
        for (int i = 0; i < granadas.Length; i++)
        {
            if (granadas[i] != null)
                granadas[i].SetActive(i < numGranadas);
        }
    }

    void UpdatePuntos()
    {
        if (textMesh != null)
            textMesh.text = $"Puntos: {puntos}";
    }

    void RefreshAll()
    {
        UpdateHearts();
        UpdateGranadas();
        UpdatePuntos();
    }

    void ClampAll()
    {
        vidas       = Mathf.Clamp(vidas, 0, vidasMax);
        numGranadas = Mathf.Clamp(numGranadas, 0, numGranadasMax);
    }

    [ContextMenu("TryAutoBindUI")]
    public void TryAutoBindUI()
    {
        // Text "Puntaje"
        if (!textMesh)
        {
            var go = GameObject.Find("Puntaje");
            if (go) textMesh = go.GetComponent<TextMeshProUGUI>();
        }

        // Hearts
        var heartsGO = GameObject.Find("Hearts");
        if (heartsGO)
        {
            int n = heartsGO.transform.childCount;
            hearts = new GameObject[n];
            for (int i = 0; i < n; i++)
                hearts[i] = heartsGO.transform.GetChild(i).gameObject;

            vidasMax = n;
            vidas = Mathf.Clamp(vidas, 0, vidasMax);
        }

        // Granadas
        var grenGO = GameObject.Find("Granadas");
        if (grenGO)
        {
            int n = grenGO.transform.childCount;
            granadas = new GameObject[n];
            for (int i = 0; i < n; i++)
                granadas[i] = grenGO.transform.GetChild(i).gameObject;

            numGranadasMax = n;
            numGranadas = Mathf.Clamp(numGranadas, 0, numGranadasMax);
        }
    }

    // ============================================
    //  🔥 RESET TOTAL del progreso guardado
    // ============================================
    [ContextMenu("RESET: Borrar todo el progreso guardado")]
    public void ResetearTodo()
    {
        Debug.Log("[GameManager] PROGRESO BORRADO MANUALMENTE");

        // Borrar claves principales
        PlayerPrefs.DeleteKey(KEY_VIDAS);
        PlayerPrefs.DeleteKey(KEY_GRANADAS);
        PlayerPrefs.DeleteKey(KEY_PUNTOS);
        PlayerPrefs.DeleteKey(KEY_WEAPON);

        // Borrar armas compradas
        PlayerPrefs.DeleteKey("OWN_WEAPON_0");
        PlayerPrefs.DeleteKey("OWN_WEAPON_1");
        PlayerPrefs.DeleteKey("OWN_WEAPON_2");

        PlayerPrefs.Save();

        // Restaurar valores iniciales en memoria
        vidas = vidasMax;
        numGranadas = numGranadasMax;
        puntos = 0;
        armaSeleccionada = WeaponType.Arma1;

        RefreshAll();
    }
}
