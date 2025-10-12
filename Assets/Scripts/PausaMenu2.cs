using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PausaMenu2 : MonoBehaviour
{
    public GameObject pauseMenuPanel;
    public GameObject volumeOptionsPanel;
    

    public static bool isPaused = false;

    private Pausa pausaControls;

    private void OnEnable()
    {
        if (pausaControls == null)
        {
            pausaControls = new Pausa();
        }

        pausaControls.UI.Enable();
        pausaControls.UI.Pausar.performed += ctx => TogglePause();
    }

    private void OnDisable()
    {
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
        // Este método ahora funciona para regresar desde cualquier panel de submenú
        volumeOptionsPanel.SetActive(false);

        pauseMenuPanel.SetActive(true);
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu RICARDO");
    }
}
