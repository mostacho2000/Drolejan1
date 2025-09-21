using UnityEngine;
using UnityEngine.UI; // Cambia el using para usar el namespace de la UI clásica

public class FPSCounter : MonoBehaviour
{
    // Cambia el tipo de variable a Text
    public Text fpsText; 
    private float updateInterval = 0.5f; 
    private float accumulatedTime = 0.0f;
    private int frames = 0;
    private float timeLeft = 0.0f;

    void Start()
    {
        if (fpsText == null)
        {
            Debug.LogError("FPS Text component is not assigned!");
            enabled = false;
            return;
        }

        timeLeft = updateInterval;
    }

    void Update()
    {
        timeLeft -= Time.deltaTime;
        accumulatedTime += Time.timeScale / Time.deltaTime;
        frames++;

        if (timeLeft <= 0.0f)
        {
            float fps = accumulatedTime / frames;
            // El resto del código es igual, ya que el método .text es el mismo
            string text = string.Format("FPS: {0:0.}", fps);
            fpsText.text = text;

            timeLeft = updateInterval;
            accumulatedTime = 0.0f;
            frames = 0;
        }
    }
}