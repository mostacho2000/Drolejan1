using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class Bala : MonoBehaviour
{

   
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
    }

   
}
