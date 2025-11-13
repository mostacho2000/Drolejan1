using UnityEngine;
using UnityEngine.UI; // ¡Importante para manejar botones!
using UnityEngine.SceneManagement;

public class SelectorNiveles : MonoBehaviour
{
    // Arrastra tus botones desde la jerarquía hasta estos campos en el Inspector
    public Button botonNivel2;
    public Button botonNivel3;

    private int nivelAlcanzado;

    void Start()
    {
        // Obtenemos el nivel más alto guardado. Si no hay nada, '1' es el valor por defecto.
        nivelAlcanzado = PlayerPrefs.GetInt("nivelAlcanzado", 1);

        // Habilitamos o deshabilitamos los botones según corresponda

        // El Nivel 1 siempre está activo (no necesitamos una variable para él)

        // El botón del Nivel 2 sólo es 'interactable' (se puede pulsar)
        // si el nivel alcanzado es 2 o más.
        if (botonNivel2 != null)
        {
            botonNivel2.interactable = (nivelAlcanzado >= 2);
        }

        // El botón del Nivel 3 sólo es 'interactable'
        // si el nivel alcanzado es 3 o más.
        if (botonNivel3 != null)
        {
            botonNivel3.interactable = (nivelAlcanzado >= 3);
        }
    }

    // --- Funciones para tus botones ---
    // Estas funciones las asignas al evento 'OnClick()' de cada botón en el Inspector.

    public void CargarNivel1()
    {
        // Reemplaza "Nivel1" con el nombre real de tu escena
        SceneManager.LoadScene("ProyectoFinal");
    }

    public void CargarNivel2()
    {
        // Reemplaza "Nivel2" con el nombre real de tu escena
        SceneManager.LoadScene("NivelMuelle");
    }

    public void CargarNivel3()
    {
        // Reemplaza "Nivel3" con el nombre real de tu escena
        SceneManager.LoadScene("NivelFinal");
    }

    // --- (OPCIONAL) Botón para borrar progreso (para pruebas) ---
    public void BorrarProgreso()
    {
        PlayerPrefs.DeleteKey("nivelAlcanzado");
        // Vuelve a cargar la escena del menú para ver los cambios
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}