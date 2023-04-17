using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class balaEnemy2 : MonoBehaviour
{
    public Transform bala_pos;
    public float speed;
     public float velocidad =200;
    public Vector2 Fuerza;//esto es direccion




    void Start()
    {
        //bala_pos = GameObject vikingo;
        bala_pos = GameObject.Find("vikingo").transform;


       Vector2 direccion = transform.position - bala_pos.position;//, speed * Time.deltaTime);
       Fuerza = new Vector2(-direccion.x, transform.position.y);

        Fuerza.Normalize();
        Fuerza *=velocidad;
        GetComponent<Rigidbody2D>().AddForce(Fuerza);
        Invoke("Destruir_", 4);

    }


    void Update()
    {
        //movimiento
        #region
        
        
            Vector2 direccion  = Vector2.MoveTowards(transform.position, bala_pos.position, speed * Time.deltaTime);
       // transform.position =new Vector2(direccion.x, transform.position.y);


        #endregion
        //flip
        #region
        if (bala_pos.position.x > this.transform.position.x)
        {
            this.transform.localScale = new Vector2(1, 1);
        }
        else
        {
            this.transform.localScale = new Vector2(-1, 1);
        }
        #endregion
        //disparar
       /* #region
        tiempo += Time.deltaTime;
        if (tiempo >= 2)
        {
            Instantiate(bala, Insatncia.position, Quaternion.identity);
            tiempo = 0;
        }
        #endregion*/

    }
    void Destruir_()
    {
        Destroy(this.gameObject);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Destruir_();
    }
}
