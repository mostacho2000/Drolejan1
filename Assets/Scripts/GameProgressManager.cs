using UnityEngine;

public class GameProgressManager : MonoBehaviour
{
    // --- ESTA ES NUESTRA NUEVA MEMORIA ---
    // 'static' significa que se comparte entre todas las escenas.
    // Empezará en '1' cada vez que inicies el juego.
    public static int nivelAlcanzado = 1;
    // ------------------------------------

    // Esto es un "Singleton" simple. Su trabajo es evitar que este
    // objeto se destruya al cambiar de escena.
    public static GameProgressManager instance;

    void Awake()
    {
        // Si ya existe un "Cerebro"...
        if (instance != null)
        {
            // ...destruye este duplicado.
            Destroy(gameObject);
        }
        else
        {
            // ...si no, este es el original.
            instance = this;
            // Dile a Unity: "No destruyas este objeto al cargar otras escenas".
            DontDestroyOnLoad(gameObject);
        }
    }
}