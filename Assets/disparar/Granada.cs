using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Granada : MonoBehaviour
{
    public float velocidadGranada = 10;
  
    public GameObject animationExplosion;



    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("suelo"))
        {
            Instantiate(animationExplosion, transform.position, Quaternion.identity);
            Destroy(gameObject);//destruimos el punto
        }
        if (collision.gameObject.CompareTag("enemy"))
        {
            Instantiate(animationExplosion, transform.position, Quaternion.identity);
            Destroy(gameObject);//destruimos el punto
        }
        if (collision.gameObject.CompareTag("balaMuerte"))
        {
            Instantiate(animationExplosion, transform.position, Quaternion.identity);
            Destroy(gameObject);//destruimos el punto
           

        }
        if (collision.gameObject.CompareTag("balaBuena"))
        {
            Instantiate(animationExplosion, transform.position, Quaternion.identity);
            Destroy(gameObject);//destruimos el punto
        }
        if (collision.gameObject.CompareTag("granada"))
        {
            Instantiate(animationExplosion, transform.position, Quaternion.identity);
            Destroy(gameObject);//destruimos el punto
        }
        if (collision.gameObject.CompareTag("BALAFINAL"))
        {
            Instantiate(animationExplosion, transform.position, Quaternion.identity);
            Destroy(gameObject);//destruimos el punto
        }
    }

    

}
