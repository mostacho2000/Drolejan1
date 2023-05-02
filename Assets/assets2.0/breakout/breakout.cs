using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class breakout : MonoBehaviour
{
    Vector3 mousePos;
    public float minY, maxY;
    public float minX, maxX;
    public int puntos; //esto tamien se puede y tambien es un variable global, es mejor declarar siempre hasta arriba, pero se puede por que esta acomodado correctamente em la jerarquia de las llaves 
    public TextMeshProUGUI textoScore;


    void Start()
    {

    }


    void Update()
    {
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Debug.Log(mousePos);
        float posYlimited = Mathf.Clamp(mousePos.y, minY, maxY);
        float posXlimited = Mathf.Clamp(mousePos.x, minX, maxX);
        transform.position = new Vector3(posXlimited, posYlimited, 0);
    }


    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("coin"))
        {
            puntos += 1;
            //al combinar dos textos se le llama concatenacion 
            textoScore.text = "Puntos: " + puntos.ToString();//convierte de numeros a texto
            Destroy(collision.gameObject);//destruimos el punto
        }


    }
}
