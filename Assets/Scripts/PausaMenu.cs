using UnityEngine;
using UnityEngine.InputSystem; // Necesario para el Input System
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PausaMenu : MonoBehaviour
{
    public GameObject pauseMenuPanel;
    public GameObject volumeOptionsPanel;
   

    public static bool isPaused = false;

    // Referencia al Input Actions Asset "Pausa"
    private Pausa pausaControls;

    // Se llama cuando el objeto se habilita
    private void OnEnable()
    {
        // Crea una nueva instancia del Input Actions Asset
        if (pausaControls == null)
        {
            pausaControls = new Pausa();
        }

        // Habilita el Action Map "UI" y registra el método para la acción "Pausar"
        pausaControls.UI.Enable();
        pausaControls.UI.Pausar.performed += ctx => TogglePause();
    }

    // Se llama cuando el objeto se deshabilita
    private void OnDisable()
    {
        // Deshabilita el Action Map para evitar errores
        pausaControls.UI.Disable();
        pausaControls.UI.Pausar.performed -= ctx => TogglePause();
    }

    void Start()
    {
        pauseMenuPanel.SetActive(false);
        volumeOptionsPanel.SetActive(false);
        isPaused = false;
        Time.timeScale = 1f;

       
    }

    // Método que se llama cuando se presiona la tecla de pausa
    public void TogglePause()
    {
        if (isPaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

    public void PauseGame()
    {
        pauseMenuPanel.SetActive(true);
        volumeOptionsPanel.SetActive(false);
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void ResumeGame()
    {
        pauseMenuPanel.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    public void ShowVolumeOptions()
    {
        pauseMenuPanel.SetActive(false);
        volumeOptionsPanel.SetActive(true);
    }

    public void GoBackToPauseMenu()
    {
        volumeOptionsPanel.SetActive(false);
        pauseMenuPanel.SetActive(true);
    }

    public void SetVolume(float volume)
    {
        AudioListener.volume = volume;
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu RICARDO");
    }
}