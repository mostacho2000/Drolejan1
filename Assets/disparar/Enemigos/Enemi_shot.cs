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

    void Start()
    {
        // Busca el jugador por tag "Player1"
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
    }

    void Update()
    {
        if (player_pos == null)
            return;

        float distanciaJugador = Vector2.Distance(transform.position, player_pos.position);

        if (distanciaJugador <= rangoDeteccion)
        {
            // Perseguir y disparar
            patrullando = false;
            PerseguirJugador();
            Disparar();
        }
        else
        {
            // Patrullar
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
            esperaActual += Time.deltaTime;
            if (esperaActual >= tiempoEspera)
            {
                esperando = false;
                esperaActual = 0f;
                puntoObjetivo = (puntoObjetivo == puntoDerecha) ? puntoIzquierda : puntoDerecha;
            }
            return;
        }

        // Solo mueve en X, manteniendo la Y actual
        Vector2 destino = new Vector2(puntoObjetivo.x, transform.position.y);
        // Girar sprite según dirección
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
        // Girar sprite según dirección hacia el jugador
        if (player_pos.position.x > transform.position.x)
            transform.localScale = new Vector2(1, 1);
        else if (player_pos.position.x < transform.position.x)
            transform.localScale = new Vector2(-1, 1);

        transform.position = Vector2.MoveTowards(transform.position, player_pos.position, speed * Time.deltaTime);
    }

    void Disparar()
    {
        tiempoDisparo += Time.deltaTime;
        if (tiempoDisparo >= 2f)
        {
            GameObject bullet = Instantiate(bala, Insatncia.position, Quaternion.identity);
            // Puedes agregar fuerza a la bala aquí si lo necesitas
            tiempoDisparo = 0f;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("balaBuena") || collision.gameObject.CompareTag("granada"))
        {
            Destroy(gameObject);
        }
    }
}