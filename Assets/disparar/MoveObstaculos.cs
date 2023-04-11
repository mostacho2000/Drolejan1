using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveObstaculos : MonoBehaviour
{
    Rigidbody2D rb;
    public float vel;
    
    void Start()
    {
        rb= GetComponent<Rigidbody2D>();
        //forma 1 de agregar velocidad
        //rb.velocity = new Vector2(-1, 0);//ejemplo como poner la velocidad con rigidbody facil
        //forma 2 para agregando fuerza
        //rb.AddForce(Vector2.left, ForceMode2D.Impulse);
        //forma 3 con variable publica
        rb.AddForce(Vector2.left*vel, ForceMode2D.Impulse);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("limite"))
        {
            Destroy(gameObject);
        }
    }
}
