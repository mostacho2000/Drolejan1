using UnityEngine;
using TMPro; // Necesario para usar TextMeshPro

public class ControladorFPS : MonoBehaviour
{
    // Referencia pública al texto que mostrará los FPS.
    // La asignaremos desde el inspector de Unity.
    public TextMeshProUGUI textoFPS;

    // Variables para promediar el cálculo de FPS y no actualizarlo en cada fotograma.
    private float tiempoAcumulado = 0.0f;
    private int fotogramasAcumulados = 0;
    private float intervaloActualizacion = 0.5f; // Actualizar cada medio segundo

    void Start()
    {
        // Es una buena práctica asegurarse de que el texto está asignado.
        if (textoFPS == null)
        {
            Debug.LogError("No se ha asignado el componente TextMeshProUGUI para los FPS.");
            // Desactivamos el script si no hay texto para evitar errores.
            this.enabled = false; 
            return;
        }

        // Ocultamos el texto de los FPS al iniciar el juego.
        textoFPS.gameObject.SetActive(false);
    }

    void Update()
    {
        // Acumulamos el tiempo y los fotogramas.
        // Usamos unscaledDeltaTime para que no se vea afectado por cambios en la escala de tiempo (ej. slow motion).
        tiempoAcumulado += Time.unscaledDeltaTime;
        fotogramasAcumulados++;

        // Si ha pasado el intervalo de tiempo definido...
        if (tiempoAcumulado > intervaloActualizacion)
        {
            // Calculamos los FPS promediados durante ese intervalo.
            float fps = fotogramasAcumulados / tiempoAcumulado;
            
            // Formateamos el texto para mostrar solo dos decimales.
            string textoResultado = string.Format("{0:F2} FPS", fps);
            
            // Actualizamos el contenido del texto en la UI.
            textoFPS.text = textoResultado;

            // Reiniciamos los contadores para el próximo intervalo.
            tiempoAcumulado = 0.0f;
            fotogramasAcumulados = 0;
        }
    }

    /// <summary>
    /// Este es el método PÚBLICO que llamará nuestro botón.
    /// Activa o desactiva el objeto de texto de los FPS.
    /// </summary>
    public void AlternarVisibilidadFPS()
    {
        // Obtenemos el estado actual del objeto (activo o inactivo).
        bool estadoActual = textoFPS.gameObject.activeSelf;
        
        // Asignamos el estado contrario para alternar su visibilidad.
        textoFPS.gameObject.SetActive(!estadoActual);
    }
}