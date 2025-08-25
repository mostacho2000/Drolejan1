using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pongControl : MonoBehaviour
{
   public  GameObject pelota;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButtonDown("Fire1"))
        {
            Instantiate(pelota,transform.position,Quaternion.identity);//crea pelota en mi posicion
        }
    }
}
