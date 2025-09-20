using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

/// <summary>
/// GameManager central: vidas, granadas, puntos y UI.
/// Persistente entre escenas. Re-enlaza la UI automáticamente al cargar cada escena.
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager instancia;

    [Header("UI (asígnalos o deja que TryAutoBindUI los encuentre)")]
    [SerializeField] private TextMeshProUGUI textMesh; // Canvas > Puntaje (TMP)
    [SerializeField] private GameObject[] hearts;      // Canvas > Hearts (hijos)
    [SerializeField] private GameObject[] granadas;    // Canvas > Granadas (hijos)

    [Header("Valores iniciales")]
    public int vidasMax = 3;
    public int vidas    = 3;

    public int numGranadasMax = 3;
    public int numGranadas    = 3;

    public int puntos = 0;

    [Header("Persistencia / Escenas")]
    [SerializeField] private bool dontDestroyOnLoad = true;
    [SerializeField] private string gameOverSceneName = "";

    // ===== Ciclo de vida =====
    private void Awake()
    {
        if (instancia != null && instancia != this)
        {
            Destroy(gameObject);
            return;
        }
        instancia = this;

        if (dontDestroyOnLoad)
            DontDestroyOnLoad(gameObject);

        // Intento de enlazar UI por si ya existe en esta escena
        TryAutoBindUI();

        // Clamps y refresco inicial
        vidas        = Mathf.Clamp(vidas,        0, vidasMax);
        numGranadas  = Mathf.Clamp(numGranadas,  0, numGranadasMax);

        UpdateHearts();
        UpdateGranadas();
        ActualizarPuntos();
    }

    private void OnEnable()  => SceneManager.sceneLoaded += OnSceneLoaded;
    private void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Cuando cambie de escena (nivel/tienda), vuelve a enlazar la UI de esa escena
        TryAutoBindUI();
        UpdateHearts();
        UpdateGranadas();
        ActualizarPuntos();
    }

    // ===== API Pública =====
    public void CambiarVidas(int delta = -1)
    {
        vidas = Mathf.Clamp(vidas + delta, 0, vidasMax);
        UpdateHearts();
        if (vidas <= 0) GameOverr();
    }

    public void SetVidas(int value)
    {
        vidas = Mathf.Clamp(value, 0, vidasMax);
        UpdateHearts();
        if (vidas <= 0) GameOverr();
    }

    public void CambiarGranadas(int delta = -1)
    {
        numGranadas = Mathf.Clamp(numGranadas + delta, 0, numGranadasMax);
        UpdateGranadas();
    }

    public bool TieneGranadas() => numGranadas > 0;

    public void CambiarPuntos(int delta)
    {
        puntos += delta;
        ActualizarPuntos();
    }

    // Alias por compatibilidad con scripts viejos
    public void AddScore(int amount) => CambiarPuntos(amount);

    public void ActualizarPuntos()
    {
        if (textMesh)
            textMesh.text = "Puntos: " + puntos;
    }

    public void GameOverr()
    {
        Debug.Log("[GameManager] GAME OVER");
        if (!string.IsNullOrEmpty(gameOverSceneName))
        {
            SceneManager.LoadScene(gameOverSceneName);
        }
        else
        {
            var idx = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadScene(idx);
        }
    }

    // ===== UI interna =====
    private void UpdateHearts()
    {
        if (hearts == null) return;
        for (int i = 0; i < hearts.Length; i++)
            if (hearts[i] != null) hearts[i].SetActive(i < vidas);
    }

    private void UpdateGranadas()
    {
        if (granadas == null) return;
        for (int i = 0; i < granadas.Length; i++)
            if (granadas[i] != null) granadas[i].SetActive(i < numGranadas);
    }

    // ===== Auto-bind de UI por escena =====
    /// <summary>
    /// Intenta localizar los objetos de UI por nombre en la escena actual.
    /// Requiere que existan: "Puntaje" (TMP), "Hearts" y "Granadas" (con hijos).
    /// </summary>
    [ContextMenu("TryAutoBindUI")]
    public void TryAutoBindUI()
    {
        // Puntaje (TMP)
        var puntajeGO = GameObject.Find("Puntaje");
        if (puntajeGO)
            textMesh = puntajeGO.GetComponent<TextMeshProUGUI>();

        // Granadas (hijos)
        var granadasGO = GameObject.Find("Granadas");
        if (granadasGO)
        {
            int n = granadasGO.transform.childCount;
            granadas = new GameObject[n];
            for (int i = 0; i < n; i++)
                granadas[i] = granadasGO.transform.GetChild(i).gameObject;

            numGranadasMax = n;
            numGranadas = Mathf.Clamp(numGranadas, 0, numGranadasMax);
        }

        // Hearts (hijos)
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
    }

    // ===== Helpers en Editor =====
    [ContextMenu("Refrescar UI (Editor)")]
    private void ContextRefresh()
    {
        UpdateHearts();
        UpdateGranadas();
        ActualizarPuntos();
    }
}
