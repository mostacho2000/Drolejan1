using UnityEngine;
using UnityEngine.SceneManagement;

public class NivelCompletado : MonoBehaviour
{
    // Esto lo pones en el Inspector.
    // Si estás en Nivel 1, pones '2'.
    // Si estás en Nivel 2, pones '3'.
    public int nivelADesbloquear;

    // Esta función se llama cuando el jugador gana (por ejemplo, al entrar en un Trigger)
    public void CompletarNivel()
    {
        // Revisamos cuál es el nivel más alto desbloqueado hasta AHORA
        // El '1' es el valor por defecto si nunca se ha guardado nada (el Nivel 1 siempre está desbloqueado)
        int nivelActual = PlayerPrefs.GetInt("nivelAlcanzado", 1);

        // Si el nuevo nivel que estamos desbloqueando es MAYOR que el que ya teníamos...
        // (Esto evita que si te pasas el Nivel 1 otra vez, se reinicie tu progreso)
        if (nivelADesbloquear > nivelActual)
        {
            // ...lo guardamos como el nuevo nivel más alto.
            PlayerPrefs.SetInt("nivelAlcanzado", nivelADesbloquear);
            PlayerPrefs.Save(); // Forzamos que se guarde el dato
        }

        // Opcional: Cargar la escena del menú de niveles
        // Reemplaza "MenuNiveles" con el nombre real de tu escena de menú
        SceneManager.LoadScene("MenuNiveles");
    }

    // Ejemplo de cómo llamarlo si usas un Trigger:
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // Asegúrate de que tu jugador tenga el Tag "Player"
        {
            CompletarNivel();
        }
    }
}