using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SelectorNiveles : MonoBehaviour
{
    public Button botonNivel2;
    public Button botonNivel3;
    private int nivelAlcanzado;

    void Start()
    {
        // --- CAMBIO AQUÍ ---
        // Ya no leemos de PlayerPrefs. Leemos de nuestra variable estática.
        nivelAlcanzado = GameProgressManager.nivelAlcanzado;

        Debug.Log("Valor LEÍDO de GameProgressManager: " + nivelAlcanzado);
        // --- FIN DEL CAMBIO ---

        if (botonNivel2 != null)
        {
            botonNivel2.interactable = (nivelAlcanzado >= 2);
        }
        if (botonNivel3 != null)
        {
            botonNivel3.interactable = (nivelAlcanzado >= 3);
        }
    }

    // --- ¡YA NO NECESITAS ESTO! ---
    // Puedes borrar toda la función BorrarProgreso().
    // El progreso se borra solo al cerrar el juego.
    /*
    public void BorrarProgreso()
    {
        // PlayerPrefs.DeleteKey("nivelAlcanzado");  <-- BORRAR
        // PlayerPrefs.Save();
        // SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    */

    // ... tus funciones de CargarNivel (esas se quedan igual) ...
    public void CargarNivel1() { SceneManager.LoadScene("ProyectoFinal"); }
    public void CargarNivel2() { SceneManager.LoadScene("NivelMuelle"); }
    public void CargarNivel3() { SceneManager.LoadScene("NivelFinal"); }
}