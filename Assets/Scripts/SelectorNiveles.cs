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
        // Leemos el nivel alcanzado (como ya lo hacías)
        nivelAlcanzado = GameProgressManager.nivelAlcanzado;

        if (botonNivel2 != null)
            botonNivel2.interactable = (nivelAlcanzado >= 2);

        if (botonNivel3 != null)
            botonNivel3.interactable = (nivelAlcanzado >= 3);
    }

    // 🔥 IMPORTANTE: cada que entras a un nivel, reiniciamos vidas/granadas/puntos
    public void CargarNivel1()
    {
        if (GameManager.instancia != null)
            GameManager.instancia.NuevoJuego();   // 3 vidas, 3 granadas, puntos = 0

        SceneManager.LoadScene("ProyectoFinal");
    }

    public void CargarNivel2()
    {
        if (GameManager.instancia != null)
            GameManager.instancia.NuevoJuego();

        SceneManager.LoadScene("NivelMuelle");
    }

    public void CargarNivel3()
    {
        if (GameManager.instancia != null)
            GameManager.instancia.NuevoJuego();

        SceneManager.LoadScene("NivelFinal");
    }
}
