using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider2D))]
public class TPtienda : MonoBehaviour
{
    [Header("Destino")]
    [SerializeField] bool loadNextByBuildIndex = false;     // si true, carga la siguiente escena del Build Settings
    [SerializeField] string sceneToLoad = "Vendedor Ricardo 1";
    [SerializeField] float delay = 0f;                      // retardo opcional antes de cargar (para FX/fade)

    [Header("Quién lo activa")]
    [SerializeField] string[] acceptedTags = { "Player", "Player1" };

    bool loading;

    // Auto-ajuste cuando añades el script
    void Reset()
    {
        var col = GetComponent<Collider2D>();
        if (col) col.isTrigger = true;                      // típico para zona de TP
    }

    // Usar TRIGGER (recomendado)
    void OnTriggerEnter2D(Collider2D other) => TryLoad(other.gameObject);

    // Soporte por si prefieres colisión (Is Trigger desactivado)
    void OnCollisionEnter2D(Collision2D col) => TryLoad(col.gameObject);

    void TryLoad(GameObject go)
    {
        if (loading) return;                                // evita doble carga
        if (!IsAcceptedTag(go.tag)) return;

        loading = true;

        if (delay > 0f) Invoke(nameof(DoLoad), delay);
        else DoLoad();
    }

    bool IsAcceptedTag(string tag)
    {
        foreach (var t in acceptedTags)
            if (!string.IsNullOrEmpty(t) && tag == t) return true;
        return false;
    }

    void DoLoad()
    {
        if (loadNextByBuildIndex)
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        else
            SceneManager.LoadScene(sceneToLoad);
    }
}
