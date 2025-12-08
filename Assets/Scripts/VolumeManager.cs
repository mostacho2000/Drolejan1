using UnityEngine;
using UnityEngine.UI;

public class VolumeManager : MonoBehaviour
{
    [SerializeField] Slider volumeSlider;

    private void Start()
    {
        // 1. Cargar el volumen guardado. Si no existe, usa 1f por defecto.
        // PlayerPrefs.GetFloat permite un segundo parametro como valor por defecto,
        // ahorrándote el "if (!PlayerPrefs.HasKey...)"
        float savedVolume = PlayerPrefs.GetFloat("musicVolume", 1f);

        // 2. Asignar el valor al slider visualmente
        volumeSlider.value = savedVolume;

        // 3. IMPORTANTE: Aplicar el volumen al AudioListener inmediatamente
        AudioListener.volume = savedVolume;
    }

    // Esta función debe llamarse cada vez que el slider se mueva
    public void ChangeVolume()
    {
        AudioListener.volume = volumeSlider.value;
        Save();
    }

    private void Save()
    {
        PlayerPrefs.SetFloat("musicVolume", volumeSlider.value);
        PlayerPrefs.Save(); // Es buena práctica forzar el guardado
    }
}