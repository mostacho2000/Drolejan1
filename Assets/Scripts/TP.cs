using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TP : MonoBehaviour
{
    [Header("A quién detecto")]
    [SerializeField] string playerTag = "Player1";  // acepta Player1 o Player

    [Header("A dónde voy")]
    [SerializeField] bool loadNextByBuildIndex = false; // si true -> carga la siguiente en Build Settings
    [SerializeField] string sceneToLoad = "Vendedor Ricardo"; // si loadNextByBuildIndex = false

    [Header("Opcional")]
    [SerializeField] float delay = 0f; // retardo antes de cargar (para FX, fade, etc.)

    bool loading;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (loading) return;
        if (!other.CompareTag(playerTag) && !other.CompareTag("Player")) return;

        loading = true;
        StartCoroutine(LoadSceneRoutine());
    }

    IEnumerator LoadSceneRoutine()
    {
        if (delay > 0f) yield return new WaitForSeconds(delay);

        if (loadNextByBuildIndex)
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        else if (!string.IsNullOrEmpty(sceneToLoad))
            SceneManager.LoadScene(sceneToLoad);
    }
}
