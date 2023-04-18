using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemi_shot : MonoBehaviour
{
    public Transform player_pos;
    public float speed;
    public float distancia_frenado;
    public float distancia_retraso;

    public Transform Insatncia;
    public GameObject bala;
    private float tiempo;
    public float Fuerza;
    
    void Start()
    {
        player_pos = GameObject.Find("vikingo").transform;
    }


    void Update()
    {
        //movimiento
        #region
        if (Vector2.Distance(transform.position, player_pos.position) > distancia_frenado)
        { 
        transform.position = Vector2.MoveTowards(transform.position, player_pos.position, speed * Time.deltaTime);
        }
        if (Vector2.Distance(transform.position, player_pos.position) < distancia_retraso)
        {
            transform.position = Vector2.MoveTowards(transform.position, player_pos.position, -speed * Time.deltaTime);
        }
        if (Vector2.Distance(transform.position, player_pos.position) < distancia_frenado&& Vector2.Distance(transform.position, player_pos.position) > distancia_retraso)
        {
            transform.position = transform.position;
        }
        #endregion
        //flip
        #region
        if (player_pos.position.x > this.transform.position.x)
        {
            this.transform.localScale = new Vector2(1, 1);
        }
        else
        {
            this.transform.localScale = new Vector2(-1, 1);
        }
        #endregion
        //disparar
        #region
        tiempo += Time.deltaTime;
        if (tiempo >= 2)
        {

            GameObject bullet = Instantiate(bala, Insatncia.position, Quaternion.identity);
           // bullet.GetComponent<Rigidbody2D>().AddForce(Fuerza);
                
            tiempo = 0;
        }
        #endregion

    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("balaBuena"))
        {
           
            Destroy(gameObject);//destruimos el punto
        }
        if (collision.gameObject.CompareTag("granada"))
        {

            Destroy(gameObject);//destruimos el punto
        }


    }
}
