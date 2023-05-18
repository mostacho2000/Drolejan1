using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class Bala : MonoBehaviour
{
    public Transform bala_pos;
    public float velocidad = 1000;
    public Vector2 Fuerza;



    void Start()
    {
        //bala_pos = GameObject FinalPlayer;
        bala_pos = GameObject.Find("FinalPlayer").transform;


        Vector2 direccion = transform.position - bala_pos.position;//, speed * Time.deltaTime);
        Fuerza = new Vector2(-direccion.x, transform.position.y);

        Fuerza.Normalize();
        Fuerza *= velocidad;
        GetComponent<Rigidbody2D>().AddForce(Fuerza);
        Invoke("Destruir_", 4);

    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("suelo"))
        {
            Destroy(gameObject);//destruimos el punto
        }
        if (collision.gameObject.CompareTag("enemy"))
        {

            Destroy(gameObject);
        }
        if (collision.gameObject.CompareTag("balaMuerte"))
        {

            Destroy(gameObject);
        }
        if (collision.gameObject.CompareTag("balaBuena"))
        {

            Destroy(gameObject);
        }
        if (collision.gameObject.CompareTag("granada"))
        {

            Destroy(gameObject);
        }
        if (collision.gameObject.CompareTag("techo"))
        {

            Destroy(gameObject);
        }
        if (collision.gameObject.CompareTag("BALAFINAL"))
        {

            Destroy(gameObject);
        }
        if (collision.gameObject.CompareTag("JEFEASESINO"))
        {

            Destroy(gameObject);
        }
        if (collision.gameObject.CompareTag("Tienda"))
        {

            Destroy(gameObject);
        }

    }
    /*private void OnTriggerEnter2D(Collider2D collision)
    {
       
    }*/
    void Update()
    {
        if (bala_pos.position.x > this.transform.position.x)
        {
            this.transform.localScale = new Vector2(-1, 1);
        }
        else
        {
            this.transform.localScale = new Vector2(1, 1);
        }
    }


}
