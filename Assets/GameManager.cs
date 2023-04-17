using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject mainMenu;
    public AudioSource musica;
    // Start is called before the first frame update   
    void Start()
    {
        Time.timeScale = 1;//aqui esta congelado el tiempo
        //esto detiene las fisicas y el juego en general pero no detiene los inputs ni los cambios en el object
    }

    void Update()
    {

    }
   /* public void Comenzar()
    {
        mainMenu.SetActive(false);
        Time.timeScale = 1;//aqui vuelve a correr el tiempo
       //musica.Play();
    }
    public void cargarNivel()
    {
        SceneManager.LoadScene("parcial 3");
    }*/
}
