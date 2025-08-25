using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class pongControler : MonoBehaviour
{
    Vector3 mousePos;
    public float minY, maxY;
    public float minX, maxX;
   

    void Start()
    {
        
    }

    
    void Update()
    {
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Debug.Log(mousePos);
        float posYlimited = Mathf.Clamp(mousePos.y, minY, maxY);
        float posXlimited = Mathf.Clamp(mousePos.x, minX, maxX);
        transform.position = new Vector3(posXlimited, posYlimited ,0);
    }
}
