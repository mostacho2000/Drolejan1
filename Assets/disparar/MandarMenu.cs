using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MandarMenu : MonoBehaviour
{
    public TextMeshProUGUI textoReinciar;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void reiniciarr()//esto se le ayade al boton de restart
    {


        Time.timeScale = 1;



        SceneManager.LoadScene("GameOver Ricardo");

    }
}
