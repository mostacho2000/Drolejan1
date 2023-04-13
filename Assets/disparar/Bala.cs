using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bala : MonoBehaviour
{
    public float velocidadBala = 10;
    public Rigidbody2D rb;
   

  
    void Update()
    {
        rb.MovePosition(transform.position + Vector3.right * velocidadBala * Time.deltaTime);
            

    }
}
