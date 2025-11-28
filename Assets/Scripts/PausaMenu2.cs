using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;   // solo si usas el nuevo Input System

public class PausaMenu2 : MonoBehaviour
{
    [Header("Panels")]
    public GameObject pauseMenuPanel;      // Panel MenuPausa
    public GameObject volumeOptionsPanel;  // Panel MenuVolumen

    public static bool isPaused { get; private set; }

    void Start()
    {
        // Asegurarnos de que todo empiece sin pausa
        ResumeGame();
    }

    void Update()
    {
        // Tecla ESC para pausar / reanudar
        if (Keyboard.current != null &&
            Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (isPaused) ResumeGame();
            else PauseGame();
        }
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;

        if (pauseMenuPanel)      pauseMenuPanel.SetActive(true);
        if (volumeOptionsPanel)  volumeOptionsPanel.SetActive(false);
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;

        if (pauseMenuPanel)      pauseMenuPanel.SetActive(false);
        if (volumeOptionsPanel)  volumeOptionsPanel.SetActive(false);
    }

    public void ShowVolumeOptions()
    {
        // Ir del menú de pausa al menú de volumen
        if (pauseMenuPanel)      pauseMenuPanel.SetActive(false);
        if (volumeOptionsPanel)  volumeOptionsPanel.SetActive(true);
    }

    public void GoBackToPauseMenu()
    {
        // Regresar de volumen al menú de pausa
        if (pauseMenuPanel)      pauseMenuPanel.SetActive(true);
        if (volumeOptionsPanel)  volumeOptionsPanel.SetActive(false);
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu RICARDO");
    }
}
