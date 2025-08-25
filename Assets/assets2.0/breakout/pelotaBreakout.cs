using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class pelotaBreakout : MonoBehaviour
{
    Vector3 centro;
    Rigidbody2D rbBall;
    public float speed;//velocidad e pelota
    public int scoreP1, scoreP2;
    public TextMeshProUGUI textoP1, textoP2;
    public GameObject panelVictoriaP1, panelVictoriaP2;
    public int puntos; //esto tamien se puede y tambien es un variable global, es mejor declarar siempre hasta arriba, pero se puede por que esta acomodado correctamente em la jerarquia de las llaves 
    public TextMeshProUGUI textoScore;
    void Start()
    {
        centro = Vector3.zero;//este es el Vector3 (0,0,0)
        transform.position = centro;
        rbBall = GetComponent<Rigidbody2D>();
    }


    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            //Inicamos el juego
            rbBall.linearVelocity = new Vector2(1, 1) * speed;

        }
        if (scoreP1 >= 5)
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
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("coin"))
        {
            puntos += 1;
            //al combinar dos textos se le llama concatenacion 
            textoScore.text = "Puntos: " + puntos.ToString();//convierte de numeros a texto
            Destroy(collision.gameObject);//destruimos el punto
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

        textoP1.text = scoreP1.ToString();
        textoP2.text = scoreP2.ToString();
    }

   
}
