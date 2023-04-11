using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class TimeControler : MonoBehaviour//el tiempo siempre va aen un objeto vacio como este 
{
    public float duracion;//lo que dura el timer
    public float tiempoActual;//el tiempo corriendo
    public TextMeshProUGUI textoTimer;

    void Start()
    {
        tiempoActual = duracion;

    }


    void Update()
    {
        if (tiempoActual > 0)
        {
            tiempoActual -= Time.deltaTime;//lo restamos el tiempo real 

        }
        textoTimer.text    = "Tiempo:" + Mathf.RoundToInt(tiempoActual).ToString();  // esto es para que no se vean los decimales al momento de que corra el reloj hacia atras 

       

        if (Input.GetKeyDown(KeyCode.R))//resetear el timer
        {
            tiempoActual = duracion;
        }

        if (tiempoActual <= 0)
        {
            SceneManager.LoadScene("GameOver");
        }





    }

}
