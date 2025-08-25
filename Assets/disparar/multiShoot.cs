using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class multiShoot : MonoBehaviour
{//usar diferente tipo de disparo
    Rigidbody2D rb;
    Vector2 mousePos;
    float movX, movY;

    public GameObject balaNueva;
    public float bulletForce;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

   
    void Update()
    {
        //Para el movimiento
        movX = Input.GetAxis("Horizontal");
        movY = Input.GetAxis("Vertical");
        rb.linearVelocity = new Vector2(movX, movY);


        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameObject labala = Instantiate(balaNueva, transform.position, Quaternion.identity);
                Rigidbody2D balarb = labala.GetComponent<Rigidbody2D>();
            balarb.AddForce(transform.right * bulletForce, ForceMode2D.Impulse);

        }
    }
    private void FixedUpdate()
    {
        //paso 1 calcular la direccion adonde aunta el mouse
        Vector2 lookDir = mousePos - rb.position;
        //paso 2 calcular el angulo d ela direccion y convertirlo a grados
        float angulo = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg;
        //paso 3 rotar el objeto al angulo calculado
        rb.rotation = angulo;
    }
}
