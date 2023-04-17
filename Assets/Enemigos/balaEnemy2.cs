using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class balaEnemy2 : MonoBehaviour
{
    public Transform bala_pos;
    public float speed;
  




    void Start()
    {
        
        bala_pos = GameObject.Find("vikingo").transform;
        Invoke("Destruir_", 4);
    }


    void Update()
    {
        //movimiento
        #region
        
        
            transform.position = Vector2.MoveTowards(transform.position, bala_pos.position, speed * Time.deltaTime);
        
       
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
