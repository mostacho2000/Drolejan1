using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;

public class ballPong : MonoBehaviour
{
    Vector3 centro;
    Rigidbody2D rbBall;
    public float speed;//velocidad e pelota
    public int scoreP1, scoreP2;
    public TextMeshProUGUI textoP1, textoP2;
    public GameObject panelVictoriaP1, panelVictoriaP2;
    void Start()
    {
        centro = Vector3.zero;//este es el Vector3 (0,0,0)
        transform.position = centro;
        rbBall = GetComponent<Rigidbody2D>();
    }

    
    void Update()
    {
        if(Input.GetButtonDown("Fire1"))
        {
            //Inicamos el juego
            rbBall.velocity = new Vector2(1, 1)*speed;

        }
        if(scoreP1 >= 5)
        {
            Time.timeScale = 0;
            panelVictoriaP1.SetActive(true);


        }
        if (scoreP2 >= 5)
        {
            Time.timeScale = 0;
            panelVictoriaP2.SetActive(true);

        }
    }
    private void OnTriggerEnter2D(Collider2D Collision)
    {
        if (Collision.gameObject.CompareTag("porteriaPlayer2"))
         {
            transform.position = centro;
            rbBall.velocity = Vector2.zero;
            //Aqui sumariamos puntos al player 1
            scoreP1++;
            textoP1 .text= scoreP1.ToString();
        }
        else if (Collision.gameObject.CompareTag("porteriaPlayer1"))
        {

            transform.position = centro;
            rbBall.velocity = Vector2.zero;
            //Aqui sumariamos puntos al player 2
            scoreP2++;
            textoP2.text = scoreP2.ToString();

        }

    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
       //aqui aplicaremos un rebote mas avanzado
    }
    public void reiniciar()//esto se le ayade al boton de restart
    {
        scoreP1 = 0;
        scoreP2 = 0;
        Time.timeScale = 1;
        panelVictoriaP1.SetActive(false);
        panelVictoriaP2.SetActive(false);

        textoP1.text=scoreP1.ToString();
        textoP2.text = scoreP2.ToString();
    }
}
