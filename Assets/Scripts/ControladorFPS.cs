using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PausaMenu : MonoBehaviour
{
    public GameObject pauseMenuPanel;
    public GameObject volumeOptionsPanel;
    
    public static bool isPaused = false;
    private Pausa pausaControls;

    // Es mejor práctica inicializar los controles en Awake()
    private void Awake()
    {
        pausaControls = new Pausa();
    }

    // --- CÓDIGO CORREGIDO Y ROBUSTO ---
    private void OnEnable()
    {
        pausaControls.UI.Enable();
        // Suscribimos el método dedicado "HandlePauseInput"
        pausaControls.UI.Pausar.performed += HandlePauseInput;
    }

    private void OnDisable()
    {
        // Desuscribimos el MISMO método dedicado, asegurando que se elimine correctamente
        pausaControls.UI.Pausar.performed -= HandlePauseInput;
        pausaControls.UI.Disable();
    }

    // Este es nuestro método dedicado para manejar la entrada de pausa
    private void HandlePauseInput(InputAction.CallbackContext context)
    {
        TogglePause();
    }
    // --- FIN DE LA CORRECCIÓN ---

    void Start()
    {
        // Asegurarse de que el juego siempre empiece sin pausa
        ResumeGame();
    }

    public void TogglePause()
    {
        // Simplificamos la lógica. Invertimos el estado y luego actuamos.
        isPaused = !isPaused; 

        if (isPaused)
        {
            PauseGame();
        }
        else
        {
            ResumeGame();
        }
    }

    public void PauseGame()
    {
        pauseMenuPanel.SetActive(true);
        volumeOptionsPanel.SetActive(false);
        
        Time.timeScale = 0f;
        isPaused = true; // Aseguramos el estado
    }

    public void ResumeGame()
    {
        pauseMenuPanel.SetActive(false);
        volumeOptionsPanel.SetActive(false); // También oculta este panel al reanudar
        
        Time.timeScale = 1f;
        isPaused = false; // Aseguramos el estado
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

    public void GoToMainMenu()
    {
        Time.timeScale = 1f; // Siempre reanudar el tiempo antes de cambiar de escena
        SceneManager.LoadScene("Menu RICARDO");
    }
}