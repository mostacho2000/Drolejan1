using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerPDO : MonoBehaviour
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

    public int puntos; 
    public TextMeshProUGUI textoScore;
    public Transform respawnPoint;//coordenadas demi punto de respawn
    Animator animationPlayer;
    public Transform spawnBalas;
    public bool bulletCD;

    [SerializeField] TimerScript timerScript;

    private IEnumerator Start()
    {
        // Pequeño delay como tenías
        yield return new WaitForSeconds(0.1f);

        // ===== ARREGLO DEL WARNING (FindObjectOfType deprecado) =====
#if UNITY_2023_1_OR_NEWER
        controlador = GameManager.instancia ?? Object.FindFirstObjectByType<GameManager>();
#else
        controlador = GameManager.instancia ?? FindObjectOfType<GameManager>();
#endif
        // ============================================================

        //Obtenemos el componente rigidbody de nuestro objeto
        cuerpoPlayer = GetComponent<Rigidbody2D>();
        saltos = 2;
        //Obtenemos en componente animator de nuestro pl
        animationPlayer = GetComponent<Animator>();
        Ready = true;
    }

    void CongelarJugador()
    {
        Debug.Log("jugadorcongelado");
    }

    void Update()
    {
        if (Time.timeScale <= 0 || Ready == false)
            return;

        // movimiento
        float posX = Input.GetAxis("Horizontal") * velocidad;
        cuerpoPlayer.linearVelocity = new Vector2(posX, cuerpoPlayer.linearVelocity.y);

        if (posX > 0)
        {
            animationPlayer.SetBool("RUN", true);
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if (posX < 0)
        {
            animationPlayer.SetBool("RUN", true);
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else
        {
            animationPlayer.SetBool("RUN", false);
        }

        // salto
        if (Input.GetButtonDown("Jump") && saltos > 0)
        {
            animationPlayer.SetTrigger("jump");
            animationPlayer.SetBool("ground", false);
            cuerpoPlayer.AddForce(new Vector2(0, fuerzaBrinco));
            saltos -= 1;
        }

        // tecla R (timer)
        if (Input.GetKey(KeyCode.R))
        {
            // ejemplos que ya tenías
            // timerScript.Settimer(10, CongelarJugador);
            // timerScript.Settimer(10, delegate () { Debug.Log("anonimo");  });
            timerScript.Settimer(10, () => { Debug.Log("lambda"); });
        }

        // anim de ataque
        if (Input.GetButtonDown("Fire1") && bulletCD == false)
        {
            animationPlayer.SetTrigger("attack");
        }

        Shoot();
        granadaLanzar();
    }

    //este bloque se ejecuta cuando colisionamos con "algo"
    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "balaMuerte")
        {
            timerScript.addtimer(-2);
        }
    }

    public void Shoot()
    {
        if (Input.GetButtonDown("Fire1") && bulletCD == false)
        {
            GameObject tiro = Instantiate(bala, spawnBalas.position, Quaternion.identity);
            Rigidbody2D rb = tiro.GetComponent<Rigidbody2D>();
            rb.AddForce(Vector2.right * 10 * transform.localScale.x, ForceMode2D.Impulse);
            StartCoroutine(cooldownBala());
        }
    }

    IEnumerator cooldownBala()
    {
        bulletCD = true;
        yield return new WaitForSeconds(2f);
        bulletCD = false;
    }

    public void granadaLanzar()
    {
        if (controlador != null && controlador.numGranadas <= 0)
        {
            return;
        }

        if (Input.GetButtonDown("Fire2"))
        {
            GameObject tiro = Instantiate(granada, transform.position, Quaternion.identity);
            Rigidbody2D rb = tiro.GetComponent<Rigidbody2D>();
            rb.AddForce(Vector2.right * 10 * transform.localScale.x, ForceMode2D.Impulse);

            //controlador.numGranadas--;
            controlador?.CambiarGranadas();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == ("coin"))
        {
            timerScript.addtimer(2);
        }
    }
}
