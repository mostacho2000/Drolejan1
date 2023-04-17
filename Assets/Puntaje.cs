using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Puntaje : MonoBehaviour
{
    private float puntos;
    private TextMeshProUGUI textMesh;

    private void Start()
    {
        textMesh = GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
      
        textMesh.text = puntos.ToString("Puntos: "+ Mathf.RoundToInt(puntos).ToString());
    }
    public void sunarPuntos(float puntosEntrada)
    {
        puntos += puntosEntrada;
    }
}
