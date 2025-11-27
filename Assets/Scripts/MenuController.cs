using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    bool cambio;

    public void ChangeScene(string name)
    {
        // ⚠️ Cada vez que entras a un nivel desde el selector:
        if (GameProgressManager.instance != null)
        {
            GameProgressManager.instance.ResetRun();   // 3 vidas, 3 granadas
        }

        StartCoroutine(Time(name));
    }

    public void Salir()
    {
        Debug.Log("Salir...");
        Application.Quit();
    }

    IEnumerator Time(string nameScene)
    {
        yield return new WaitForSeconds(0.4f);
        SceneManager.LoadScene(nameScene);
    }

    public void inicio()
    {
        // Si usas este botón para ir directo al nivel 1:
        if (GameProgressManager.instance != null)
        {
            GameProgressManager.instance.ResetRun();   // 3 vidas, 3 granadas
        }

        SceneManager.LoadScene("GameScene2");
        Cursor.visible = cambio;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void Final()
    {
        SceneManager.LoadScene("winner");
        Cursor.visible = cambio;
        Cursor.lockState = CursorLockMode.Locked;
    }
}
