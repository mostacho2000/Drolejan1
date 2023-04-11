using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class spawnObstaculos : MonoBehaviour
{
    public GameObject obstaculo;
    Collider2D limites;
    void Start()
    {
        StartCoroutine(spawner());
        limites = GetComponent<Collider2D>();

    }
    IEnumerator spawner()
    {
        while (true)// gracias a  este while es un ciclo infinito
        {
            Debug.Log("HOLA");
            yield return new WaitForSeconds(2f);//tiempo de espera de cuanto tiempo pasa para spawnear otro
            //Vector2 posAleatoria = new Vector2(transform.position.x, Random.Range(-4.5f, 4.5f)); una forma de hacerlo
            Vector2 posAleatoria2 = new Vector2(transform.position.x, Random.Range(limites.bounds.min.y, limites.bounds.max.y));//otra forma de hacerlo
            Instantiate(obstaculo, posAleatoria2, Quaternion.identity);
        }
        
    }
   
}
