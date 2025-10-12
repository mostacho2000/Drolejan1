using UnityEngine;
using System.Collections.Generic; // Necesario para usar List<T>

[RequireComponent(typeof(Collider2D))]
public class MuroAtravesable : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField]
    private Collider2D playerCollider; // Arrastra al jugador aquí

    // Listas para guardar las referencias de todos los enemigos
    private List<Collider2D> enemyColliders = new List<Collider2D>();
    private List<Rigidbody2D> enemyRbs = new List<Rigidbody2D>();

    private Collider2D wallCollider;
    private Rigidbody2D playerRb;

    void Awake()
    {
        wallCollider = GetComponent<Collider2D>();

        // 1. Configurar al jugador (igual que antes)
        if (playerCollider != null)
        {
            playerRb = playerCollider.GetComponent<Rigidbody2D>();
        }
        else // Plan B: buscarlo por tag si no se arrastró
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player1");
            if (player != null)
            {
                playerCollider = player.GetComponent<Collider2D>();
                playerRb = player.GetComponent<Rigidbody2D>();
            }
        }

        // 2. Encontrar y configurar a TODOS los enemigos
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("enemy");
        foreach (GameObject enemy in enemies)
        {
            Collider2D enemyCol = enemy.GetComponent<Collider2D>();
            Rigidbody2D enemyBody = enemy.GetComponent<Rigidbody2D>();

            if (enemyCol != null && enemyBody != null)
            {
                enemyColliders.Add(enemyCol);
                enemyRbs.Add(enemyBody);
            }
        }
    }

    void Update()
    {
        // Procesa la lógica para el jugador
        if (playerCollider != null && playerRb != null)
        {
            ProcessPlayer(playerCollider, playerRb);
        }

        // Procesa la lógica para cada enemigo en la lista
        for (int i = 0; i < enemyColliders.Count; i++)
        {
            ProcessEnemy(enemyColliders[i], enemyRbs[i]);
        }
    }

    // Lógica específica para el JUGADOR (puede saltar y aterrizar)
    void ProcessPlayer(Collider2D targetCollider, Rigidbody2D targetRb)
    {
        // Prioridad 1: Comprobación vertical para aterrizar
        float platformTopY = wallCollider.bounds.max.y;
        float playerBottomY = targetCollider.bounds.min.y;

        if (playerBottomY >= platformTopY - 0.05f)
        {
            Physics2D.IgnoreCollision(wallCollider, targetCollider, false);
            return;
        }

        // Prioridad 2: Lógica horizontal para atravesar
        float targetVelocityX = targetRb.linearVelocity.x;
        bool shouldIgnore = (targetCollider.transform.position.x < transform.position.x && targetVelocityX > 0.1f) ||
                            (targetCollider.transform.position.x > transform.position.x && targetVelocityX < -0.1f);
        
        Physics2D.IgnoreCollision(wallCollider, targetCollider, shouldIgnore);
    }

    // Lógica simplificada para los ENEMIGOS (solo atraviesan horizontalmente)
    void ProcessEnemy(Collider2D targetCollider, Rigidbody2D targetRb)
    {
        float targetVelocityX = targetRb.linearVelocity.x;
        bool shouldIgnore = (targetCollider.transform.position.x < transform.position.x && targetVelocityX > 0f) ||
                            (targetCollider.transform.position.x > transform.position.x && targetVelocityX < 0f);

        Physics2D.IgnoreCollision(wallCollider, targetCollider, shouldIgnore);
    }
}