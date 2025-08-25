using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movimientoPelota : MonoBehaviour
{

    Rigidbody2D movimienoPelota;
    // Start is called before the first frame update

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        movimienoPelota = GetComponent<Rigidbody2D>();
        movimienoPelota.linearVelocity = new Vector2(1, 0);
    }
}
