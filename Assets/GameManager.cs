using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
   public int vidas;
   public int puntos;
   public int numGranadas;
   public TextMeshProUGUI textMesh;


    public GameObject[] hearts;
    public GameObject[] granadas;

    private void Awake()
    {
        GameManager[] curObjectScripts = FindObjectsOfType<GameManager>(true);
        if (curObjectScripts.Length > 1)
            Destroy(gameObject);
        else
            DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        UpdateGranadas();
        UpdateHearts();
        ActualizarPuntos();

    }

    public void CambiarGranadas()
    {
        numGranadas--;
        UpdateGranadas();
    }

    public void CambiarVidas()
    {
        vidas--;
        UpdateHearts();
    }

    public void CambiarPuntos(int intercambiarPuntos)
    {
       puntos+=intercambiarPuntos;
        ActualizarPuntos();
    }

    public void UpdateHearts()
    {
        if (vidas == 3)
        {
            hearts[0].gameObject.SetActive(true);
            hearts[1].gameObject.SetActive(true);
            hearts[2].gameObject.SetActive(true);
        }
        else if (vidas == 2)
        {
            hearts[0].gameObject.SetActive(true);
            hearts[1].gameObject.SetActive(true);
            hearts[2].gameObject.SetActive(false);
        }
        else if (vidas == 1)
        {

            hearts[0].gameObject.SetActive(true);
            hearts[1].gameObject.SetActive(false);
            hearts[2].gameObject.SetActive(false);
        }
        else if (vidas == 0)
        {

            SceneManager.LoadScene("GameOver Ricardo");
        }
    }
    

    public void UpdateGranadas()
    {
        if (numGranadas == 3)
        {
            granadas[0].gameObject.SetActive(true);
            granadas[1].gameObject.SetActive(true);
            granadas[2].gameObject.SetActive(true);
        }
        else if (numGranadas == 2)
        {
            granadas[0].gameObject.SetActive(true);
            granadas[1].gameObject.SetActive(true);
            granadas[2].gameObject.SetActive(false);
        }
        else if (numGranadas == 1)
        {

            granadas[0].gameObject.SetActive(true);
            granadas[1].gameObject.SetActive(false);
            granadas[2].gameObject.SetActive(false);
        }
        else if (numGranadas <= 0)
        {

            granadas[0].gameObject.SetActive(false);
            granadas[1].gameObject.SetActive(false);
            granadas[2].gameObject.SetActive(false);
        }
    }
    //public nivel de accesibilidad.
    //void tipo de la funcion (void significa que no regresa nada)
    //"sunarPuntos" es solo un nombre
    //(int puntosEntrada) es un parametro // int es solo el tipo //"puntosEntrada" es solo el nombre
    //lo que esta adentro d elas llaves{} es el contenido de la funcion
    public void ActualizarPuntos()
    {
        
        textMesh.text = puntos.ToString("Puntos: " + Mathf.RoundToInt(puntos).ToString());
    }
    
}
