using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Puntaje : MonoBehaviour
{
    GameManager controlador;
    private float puntos;
    private TextMeshProUGUI textMesh;

    private void Start()
    {
        controlador = FindObjectOfType<GameManager>();
        puntos =controlador.puntos;
        textMesh = GetComponent<TextMeshProUGUI>();
    }

     void Update()
    {
      
        textMesh.text = puntos.ToString("Puntos: "+ Mathf.RoundToInt(puntos).ToString());
    }
    public void sunarPuntos(float puntosEntrada)
    {
        puntos += puntosEntrada;
    }
}
