using UnityEngine;
using UnityEngine.SceneManagement;

public class PuertaFinalNivel : MonoBehaviour
{
    public int nivelADesbloquear;
    public string nombreEscenaACargar;
    private bool nivelCompletado = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // (Esta parte se queda igual)
        if (other.CompareTag("Player") && !nivelCompletado)
        {
            nivelCompletado = true;
            CompletarYGuardar();
        }
    }

    private void CompletarYGuardar()
    {
        // --- CAMBIO AQU� ---
        // Leemos de la variable est�tica
        int nivelActual = GameProgressManager.nivelAlcanzado;

        // Si este nivel desbloquea uno nuevo...
        if (nivelADesbloquear > nivelActual)
        {
            // ...lo "guardamos" en la variable est�tica
            GameProgressManager.nivelAlcanzado = nivelADesbloquear;

            Debug.Log("�Progreso 'guardado' en variable est�tica! Nuevo valor: " + nivelADesbloquear);
        }
        else
        {
            Debug.Log("El nivel ya estaba desbloqueado. No se 'guarda' nada nuevo.");
        }
        // --- FIN DEL CAMBIO ---

        SceneManager.LoadScene(nombreEscenaACargar);
    }
}