using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FajoBillete : MonoBehaviour
{
    GameManager controlador;
    int cantidadPuntos = 1;

    private void Start()
    {
        //controlador = FindObjectOfType<GameManager>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.instancia.CambiarPuntos(1);
           // controlador.CambiarPuntos(cantidadPuntos);
            Destroy(gameObject);
        }
    }
}
