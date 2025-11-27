using UnityEngine;

public class GameProgressManager : MonoBehaviour
{
    // --- PROGRESO DE NIVELES (LO QUE YA TENÍAS) ---
    public static int nivelAlcanzado = 1;

    // Singleton
    public static GameProgressManager instance;

    // --- NUEVO: VIDAS Y GRANADAS ---

    [Header("Valores iniciales")]
    public int defaultLives = 3;      // vidas con las que empieza cada nivel
    public int defaultGrenades = 3;   // granadas con las que empieza cada nivel

    [Header("Valores actuales (runtime)")]
    public int currentLives;
    public int currentGrenades;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        ResetRun();
    }

    // Llama a esto cuando entres a un nivel desde el selector
    public void ResetRun()
    {
        currentLives    = defaultLives;
        currentGrenades = defaultGrenades;
    }
}
