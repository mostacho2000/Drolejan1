using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControladorTienda : MonoBehaviour
{
    int precioWhyski = 1;
    int precioGranada = 1;
    GameManager controlador;
    private IEnumerator Start()
    {
        yield return new WaitForSeconds(0.1f);
        controlador = FindObjectOfType<GameManager>();

    }

    // private void Start() => controlador = FindObjectOfType<GameManager>();//wato wa lo mismo que arriba 

    public void ComprarGranada()
    {
        if (controlador.puntos<=0)
        { 
            return;
        }

        // controlador.puntos = controlador.puntos - precioGranada; //es lo mismo que abajo
        if (controlador.numGranadas < 3)
        {
            controlador.puntos -= precioGranada;
            controlador.numGranadas++;
            controlador.UpdateGranadas();//suma granadas
            controlador.ActualizarPuntos();//actualiza en el UI
        }
    }

    public void ComprarWhyski()
    {
        if (controlador.puntos <= 0)
        { 
            return ; 
        }

        if (controlador.vidas < 3)
        {
            controlador.puntos -= precioWhyski;
            controlador.vidas++;
            controlador.UpdateHearts();//suma vidas
            controlador.ActualizarPuntos();//actualiza en el UI
        }
    }
}
