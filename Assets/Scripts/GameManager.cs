using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public enum WeaponType { Arma1 = 0, Arma2 = 1, Arma3 = 2 }

public class GameManager : MonoBehaviour
{
    public static GameManager instancia;

    [Header("UI (auto-bind por nombre: Puntaje / Hearts / Granadas)")]
    [SerializeField] private TextMeshProUGUI textMesh;   // Texto "Puntos:"
    [SerializeField] private GameObject[] hearts;         // Hijos de "Hearts"
    [SerializeField] private GameObject[] granadas;       // Hijos de "Granadas"

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
    [SerializeField] private string gameOverSceneName = "";

    void Awake()
    {
        if (instancia != null && instancia != this) { Destroy(gameObject); return; }
        instancia = this;
        if (dontDestroyOnLoad) DontDestroyOnLoad(gameObject);

        // Cargar arma guardada (si existe)
        armaSeleccionada = (WeaponType)PlayerPrefs.GetInt("GM_WEAPON", 0);

        TryAutoBindUI();
        ClampAll();
        RefreshAll();
    }

    void OnEnable()  => SceneManager.sceneLoaded += OnSceneLoaded;
    void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

    void OnSceneLoaded(Scene s, LoadSceneMode m)
    {
        TryAutoBindUI();
        RefreshAll();
    }

    // =========== API de VIDAS / GRANADAS / PUNTOS ===========
    public void CambiarVidas(int delta)
    {
        vidas = Mathf.Clamp(vidas + delta, 0, vidasMax);
        UpdateHearts();
        if (vidas <= 0) GameOver();
    }

    public void SetVidas(int value)
    {
        vidas = Mathf.Clamp(value, 0, vidasMax);
        UpdateHearts();
        if (vidas <= 0) GameOver();
    }

    public void CambiarGranadas(int delta)
    {
        numGranadas = Mathf.Clamp(numGranadas + delta, 0, numGranadasMax);
        UpdateGranadas();
    }

    public bool TieneGranadas() => numGranadas > 0;

    public void CambiarPuntos(int delta)
    {
        puntos += delta;
        UpdatePuntos();
    }

    public void AddScore(int amount) => CambiarPuntos(amount);

    // =========== ARMAS / TIENDA ===========
    // ¿Alcanza para comprar?
    public bool PuedeComprar(int costo) => puntos >= costo;

    // Compra y cambia el arma; descuenta puntos; devuelve true si se pudo
    public bool ComprarArma(WeaponType arma, int costo)
    {
        if (!PuedeComprar(costo)) return false;

        puntos -= costo;
        UpdatePuntos();

        SeleccionarArma(arma);

        // Marca de propiedad (por si luego quieres checar si ya la tiene)
        PlayerPrefs.SetInt($"OWN_WEAPON_{(int)arma}", 1);
        PlayerPrefs.Save();

        return true;
    }

    // Alias si tu botón manda el arma como índice (int)
    public bool ComprarArma(int armaIndex, int costo)
        => ComprarArma((WeaponType)Mathf.Clamp(armaIndex, 0, 2), costo);

    // Selección directa (sin costo)
    public void SeleccionarArma(WeaponType arma)
    {
        armaSeleccionada = arma;
        PlayerPrefs.SetInt("GM_WEAPON", (int)armaSeleccionada);
        PlayerPrefs.Save();
    }

    // =========== GAME OVER ===========
    public void GameOver()
    {
        Debug.Log("[GameManager] GAME OVER");
        if (!string.IsNullOrEmpty(gameOverSceneName))
            SceneManager.LoadScene(gameOverSceneName);
        else
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // Compatibilidad con código viejo
    public void GameOverr() => GameOver();

    // =========== UI interna ===========
    void UpdateHearts()
    {
        if (hearts == null) return;
        for (int i = 0; i < hearts.Length; i++)
            if (hearts[i]) hearts[i].SetActive(i < vidas);
    }

    void UpdateGranadas()
    {
        if (granadas == null) return;
        for (int i = 0; i < granadas.Length; i++)
            if (granadas[i]) granadas[i].SetActive(i < numGranadas);
    }

    void UpdatePuntos()
    {
        if (textMesh) textMesh.text = "Puntos: " + puntos;
    }

    void RefreshAll()
    {
        ClampAll();
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
        // Busca por nombre en la escena para autoconectar
        var puntajeGO = GameObject.Find("Puntaje");
        if (puntajeGO) textMesh = puntajeGO.GetComponent<TextMeshProUGUI>();

        var granadasGO = GameObject.Find("Granadas");
        if (granadasGO)
        {
            int n = granadasGO.transform.childCount;
            granadas = new GameObject[n];
            for (int i = 0; i < n; i++) granadas[i] = granadasGO.transform.GetChild(i).gameObject;
            numGranadasMax = n;
            numGranadas = Mathf.Clamp(numGranadas, 0, numGranadasMax);
        }

        var heartsGO = GameObject.Find("Hearts");
        if (heartsGO)
        {
            int n = heartsGO.transform.childCount;
            hearts = new GameObject[n];
            for (int i = 0; i < n; i++) hearts[i] = heartsGO.transform.GetChild(i).gameObject;
            vidasMax = n;
            vidas = Mathf.Clamp(vidas, 0, vidasMax);
        }
    }
}
