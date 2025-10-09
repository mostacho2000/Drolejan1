using System.Collections;
using UnityEngine;

public class Enemi_shot : MonoBehaviour
{
    public Transform player_pos;
    public float speed = 2f;
    public float rangoDeteccion = 6f;
    public Transform Insatncia;
    public GameObject bala;
    private float tiempoDisparo;
    public float Fuerza;

    private Vector2 puntoIzquierda;
    private Vector2 puntoDerecha;
    private Vector2 puntoObjetivo;
    private bool patrullando = true;
    private bool esperando = false;
    private float tiempoEspera = 2f;
    private float esperaActual = 0f;

    // --- NUEVAS VARIABLES ---
    private Animator animator; // Referencia al componente Animator
    public float tiempoAnimacionMuerte = 1f; // Duración en segundos de tu animación de muerte. Ajústala en el Inspector.
    private bool isDead = false; // Para evitar que se active la muerte múltiples veces

    void Start()
    {
        // --- NUEVO ---
        // Obtenemos el componente Animator que está en el mismo GameObject
        animator = GetComponent<Animator>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player1");
        if (playerObj != null)
        {
            player_pos = playerObj.transform;
        }
        else
        {
            Debug.LogWarning("No se encontró un objeto con el tag 'Player1'");
        }

        float x = transform.position.x;
        float y = transform.position.y;
        puntoIzquierda = new Vector2(x - 5f, y);
        puntoDerecha = new Vector2(x + 5f, y);
        puntoObjetivo = puntoDerecha;
        Application.targetFrameRate = 90;
    }

    void Update()
    {
        // Si está muerto, no hace nada
        if (isDead) return;

        if (player_pos == null)
            return;

        float distanciaJugador = Vector2.Distance(transform.position, player_pos.position);

        if (distanciaJugador <= rangoDeteccion)
        {
            patrullando = false;
            PerseguirJugador();
            Disparar();
        }
        else
        {
            if (!patrullando)
            {
                patrullando = true;
                esperaActual = 0f;
            }
            Patrullar();
        }
    }

    void Patrullar()
    {
        if (esperando)
        {
            // --- NUEVO ---
            // Está esperando, así que activamos la animación Idle
            animator.SetBool("isMoving", false);

            esperaActual += Time.deltaTime;
            if (esperaActual >= tiempoEspera)
            {
                esperando = false;
                esperaActual = 0f;
                puntoObjetivo = (puntoObjetivo == puntoDerecha) ? puntoIzquierda : puntoDerecha;
            }
            return;
        }

        // --- NUEVO ---
        // Se está moviendo, así que activamos la animación de movimiento
        animator.SetBool("isMoving", true);

        Vector2 destino = new Vector2(puntoObjetivo.x, transform.position.y);
        if (destino.x > transform.position.x)
            transform.localScale = new Vector2(1, 1);
        else if (destino.x < transform.position.x)
            transform.localScale = new Vector2(-1, 1);

        transform.position = Vector2.MoveTowards(transform.position, destino, speed * Time.deltaTime);

        if (Mathf.Abs(transform.position.x - puntoObjetivo.x) < 0.05f)
        {
            esperando = true;
        }
    }

    void PerseguirJugador()
    {
        // --- NUEVO ---
        // Siempre se está moviendo al perseguir
        animator.SetBool("isMoving", true);

        if (player_pos.position.x > transform.position.x)
            transform.localScale = new Vector2(1, 1);
        else if (player_pos.position.x < transform.position.x)
            transform.localScale = new Vector2(-1, 1);

        transform.position = Vector2.MoveTowards(transform.position, player_pos.position, speed * Time.deltaTime);
    }

    void Disparar()
{
    tiempoDisparo += Time.deltaTime;
    if (tiempoDisparo >= 1f)
    {
        animator.SetTrigger("Shoot");

        // --- LÓGICA MODIFICADA ---

        // 1. Definimos la rotación por defecto (mirando a la derecha)
        Quaternion rotacionBala = Quaternion.identity;

        // 2. Comprobamos si el enemigo está mirando a la izquierda (escala en X es -1)
        if (transform.localScale.x < 0)
        {
            // Si mira a la izquierda, giramos la bala 180 grados
            rotacionBala = Quaternion.Euler(0, 180f, 0);
        }

        // 3. Instanciamos la bala con la posición y la ROTACIÓN CORRECTA
        GameObject bullet = Instantiate(bala, Insatncia.position, rotacionBala);
        
        // --- FIN DE LA MODIFICACIÓN ---

        tiempoDisparo = 0f;
    }
}
    
    // --- MODIFICADO ---
    // Cambiamos la función de colisión para que inicie la corutina de muerte
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDead) return; // Si ya está muriendo, no hacemos nada

        if (collision.gameObject.CompareTag("balaBuena") || collision.gameObject.CompareTag("granada"))
        {
            StartCoroutine(Morir());
        }
    }

    // --- NUEVA CORUTINA ---
    // Esta función maneja el proceso de muerte
    IEnumerator Morir()
    {
        isDead = true;

        // 1. Activa la animación de muerte
        animator.SetTrigger("Die");
        
        // 2. Desactiva la lógica del enemigo para que no pueda moverse ni disparar más
        this.enabled = false;
        GetComponent<Collider2D>().enabled = false; // También desactiva el collider para no recibir más daño

        // 3. Espera a que la animación de muerte termine
        yield return new WaitForSeconds(tiempoAnimacionMuerte);

        // 4. Destruye el objeto
        Destroy(gameObject);
    }
}