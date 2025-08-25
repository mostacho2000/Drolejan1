using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player2 : MonoBehaviour
{
    Transform posPelota;
    public float cpuLevel;// usar numeros entre 0.5 y 0.9
    void Start()
    {
        posPelota = GameObject.Find("Pelota").GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector2(transform.position.x, posPelota.position.y*cpuLevel);
    }
}
