using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TPlevel2 : MonoBehaviour
{
    public TextMeshProUGUI textoReinciar;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void muelle()//esto se le ayade al boton de restart
    {


        Time.timeScale = 1;



        SceneManager.LoadScene("NivelMuelle");

    }
     public void finallevel()//esto se le ayade al boton de restart
    {


        Time.timeScale = 1;



        SceneManager.LoadScene("NivelFinal");

    }
}
