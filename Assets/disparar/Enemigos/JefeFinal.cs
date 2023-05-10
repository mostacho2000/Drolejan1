using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JefeFinal : MonoBehaviour
{
    public Transform Insatncia;
    public GameObject bala;
    public int vidas;
    private float tiempo;
    void Start()
    {
    vidas =5;
    }

    // Update is called once per frame
    void Update()
    {
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

            vidas--;
        }
        if (collision.gameObject.CompareTag("granada"))
        {

            vidas--;
        }
        if (vidas==0)
        {

            Destroy(gameObject);//destruimos el punto
        }

    }
}
