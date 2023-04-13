using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Granada : MonoBehaviour
{
    public float velocidadGranada = 10;
    public Rigidbody2D rb;



    void Update()
    {
        rb.MovePosition(transform.position + Vector3.right * velocidadGranada * Time.deltaTime);


    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("suelo"))
        {
            Destroy(gameObject,3f);//destruimos el punto
        }
    }
   
}
