using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using Unity.VisualScripting;

public class playerControl : MonoBehaviour
{
    GameManager controlador;
    Rigidbody2D cuerpoPlayer;
    public GameObject bala;
    public float velocidadBala = 10;
    public GameObject granada;
    public float VelocidadDeGranada;
    public float velocidad;
    public float fuerzaBrinco;
    public TimeControler tiempo;
    int saltos;
    bool Ready;
   
    public int puntos; //esto tamien se puede y tambien es un variable global, es mejor declarar siempre hasta arriba, pero se puede por que esta acomodado correctamente em la jerarquia de las llaves 
    public TextMeshProUGUI textoScore;
    public Transform respawnPoint;//coordenadas demi punto de respawn
    Animator animationPlayer;
    public Transform spawnBalas;
    public bool bulletCD;

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(0.1f);
        controlador =FindObjectOfType<GameManager>();
        
        //Obtenemos el componente rigidbody de nuestro objeto
        cuerpoPlayer = GetComponent<Rigidbody2D>();
        saltos = 2;
        //Obtenemos en componente animator de nuestro pl
        animationPlayer = GetComponent<Animator>();
        Ready = true;
    }

    void Update()
    {
        if (Time.timeScale <= 0  || Ready == false) 
            return;
        //esto es lo del movimiento
        float posX = Input.GetAxis("Horizontal")*velocidad;
       
        cuerpoPlayer.velocity=new Vector2(posX,cuerpoPlayer.velocity.y);
        if(posX > 0)
        {
            animationPlayer.SetBool("RUN", true);
            //esto es si no escalaron a su peronsaje manualmente pero si lo hicieron en la imagen
            transform.localScale = new Vector3(1,1,1);
            //usarlo solo mi movi la escala del personaje en el scale (estiraron personaje)
            //transform.localScale = new Vector3(transform.localScale.x,transform.localScale.y,transform.localScale.z);

        }
        else if(posX < 0)
        {
            animationPlayer.SetBool("RUN", true);
            //esto es si no escalaron a su peronsaje manualmente pero si lo hicieron en la imagen
            transform.localScale = new Vector3(-1, 1, 1);
            //usarlo solo mi movi la escala del personaje en el scale (estiraron personaje)
            //transform.localScale = new Vector3(transform.localScale.x,transform.localScale.y,transform.localScale.z);

        }
        else
        {
            animationPlayer.SetBool("RUN", false);
            //aqui va la animacion de esperar
        }
        //esto es el salto
        if (Input.GetButtonDown("Jump")&&saltos>0)
        {
            //animacion de brinco
            animationPlayer.SetTrigger("jump");
            animationPlayer.SetBool("ground", false);
            cuerpoPlayer.AddForce(new Vector2(0, fuerzaBrinco));
            saltos -= 1;
            //tambien se puede poner asi: saltos--

        }
        if (Input.GetButtonDown("Fire1")&& bulletCD==false)//esto es cuando el tiempo corre se activa sino no se activa
        {
            animationPlayer.SetTrigger("attack");

        }
        //Shoot();
        granadaLanzar();
     
    }
    

    //este bloque se ejecuta cuando colisionamos con "algo"
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("suelo")) 
        {
            saltos = 2;//Recargo mis saltos al tocar el suelo
            animationPlayer.SetBool("ground", true);
        }

        // al chocar con el enemigo hago respawn al punto indicado
        if (collision.gameObject.CompareTag("balaMuerte"))
        {
            controlador.CambiarVidas();
        }
        /*if (collision.gameObject.CompareTag("VIctoria"))
        {
            SceneManager.LoadScene("Winner");
        }*/

        if (collision.gameObject.CompareTag("aguita"))
        {

            GameManager.instancia.GameOverr();
        }
        if (collision.gameObject.CompareTag("BALAFINAL"))
        {

            GameManager.instancia.GameOverr();
        }
    }
    public void Shoot()
    {
        if (bulletCD==false)
       {
            GameObject tiro=Instantiate(bala, spawnBalas.position, Quaternion.identity);
            Rigidbody2D rb = tiro.GetComponent<Rigidbody2D>();
            rb.AddForce(Vector2.right*20*transform.localScale.x,ForceMode2D.Impulse);
            StartCoroutine(cooldownBala());
        }
    }

    IEnumerator cooldownBala()
    {
        bulletCD = true;
        yield return new WaitForSeconds(1f);
        bulletCD = false;
    }

    public void granadaLanzar()
    {
        if (controlador.numGranadas <=  0)
        {
            return;
        }
        if (Input.GetButtonDown("Fire2") /*&& Time.timeScale > 0*/)
        {
            GameObject tiro = Instantiate(granada, transform.position, Quaternion.identity);
            Rigidbody2D rb = tiro.GetComponent<Rigidbody2D>();
            rb.AddForce(Vector2.right * 10 * transform.localScale.x, ForceMode2D.Impulse);
            //controlador.numGranadas--;
            controlador.CambiarGranadas();
        }
        
    }

    
    /*void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("coin"))
        {
            tiempo.tiempoActual += 2;
            puntos+=1;
            //al combinar dos textos se le llama concatenacion 
            textoScore.text = "Puntos: " + puntos.ToString();//convierte de numeros a texto
            Destroy(collision.gameObject);//destruimos el punto
        }
      

    }*/
}
