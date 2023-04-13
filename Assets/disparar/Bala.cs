using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bala : MonoBehaviour
{
    public float velocidadBala = 10;
    public Rigidbody2D rb;
    public Transform PosPlayer;

    private void Start()
    {
        PosPlayer = GameObject.Find("vikingo").GetComponent<Transform>();
    }

    void Update()
    {
        rb.MovePosition(transform.position + Vector3.right * velocidadBala * Time.deltaTime );
            

    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("suelo"))
        {
            Destroy(gameObject);//destruimos el punto
        }
    }
}
