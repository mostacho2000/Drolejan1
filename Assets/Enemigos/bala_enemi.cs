using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bala_enemi : MonoBehaviour
{
    public float spped;
    private Rigidbody2D m_rig;
    public Transform bala_pos;

    void Start()
    {
        m_rig = GetComponent<Rigidbody2D>();
     
        m_rig.AddForce(Vector2.left * spped, ForceMode2D.Impulse);

        Invoke("Destruir_", 2);
    }

    void Destruir_()
    {
        Destroy(this.gameObject);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Destruir_();
    }
    /*void Update()
    { 
        if (bala_pos.position.x < this.transform.position.x)
        {
            
            this.transform.localScale = new Vector2(1, 1);
        }
        else
        {
            
            this.transform.localScale = new Vector2(-1, 1);
        }
    }*/
}
