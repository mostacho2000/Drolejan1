using UnityEngine;
using System.Collections.Generic; // List<T>

[RequireComponent(typeof(Collider2D))]
public class MuroAtravesable : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Collider2D playerCollider;   // Arrastra el collider del player
    [SerializeField] private string playerTag = "Player1"; // Fallback si no se asigna por Inspector
    [SerializeField] private string enemyTag  = "enemy";   // Tag de enemigos

    // Listas paralelas (collider/rb) de enemigos
    private readonly List<Collider2D>  enemyColliders = new List<Collider2D>();
    private readonly List<Rigidbody2D> enemyRbs       = new List<Rigidbody2D>();

    private Collider2D  wallCollider;
    private Rigidbody2D playerRb;

    void Awake()
    {
        wallCollider = GetComponent<Collider2D>();

        // --- Player ---
        if (playerCollider)
        {
            playerRb = playerCollider.GetComponent<Rigidbody2D>();
        }
        else
        {
            var player = GameObject.FindGameObjectWithTag(playerTag);
            if (player)
            {
                playerCollider = player.GetComponent<Collider2D>();
                playerRb       = player.GetComponent<Rigidbody2D>();
            }
        }

        // --- Enemigos (semilla inicial) ---
        var enemies = GameObject.FindGameObjectsWithTag(enemyTag);
        foreach (var e in enemies)
        {
            if (!e) continue;

            // Si el collider está en un hijo, puedes usar GetComponentInChildren
            var col = e.GetComponent<Collider2D>();
            var rb  = e.GetComponent<Rigidbody2D>();

            if (col && rb)
            {
                enemyColliders.Add(col);
                enemyRbs.Add(rb);
            }
        }
    }

    void Update()
    {
        // Player seguro
        if (wallCollider && playerCollider && playerRb)
            ProcessPlayer(playerCollider, playerRb);

        // Enemigos: iterar hacia atrás y limpiar referencias destruidas
        for (int i = enemyColliders.Count - 1; i >= 0; i--)
        {
            var col = enemyColliders[i];
            var rb  = (i < enemyRbs.Count) ? enemyRbs[i] : null;

            // Si el enemigo se destruyó o perdió componentes, se elimina de las listas
            if (!col || !rb)
            {
                if (i < enemyRbs.Count) enemyRbs.RemoveAt(i);
                enemyColliders.RemoveAt(i);
                continue;
            }

            ProcessEnemy(col, rb);
        }
    }

    // -------------------- Lógica Player (salto/aterrizaje + horizontal) --------------------
    void ProcessPlayer(Collider2D targetCollider, Rigidbody2D targetRb)
    {
        if (!wallCollider || !targetCollider || !targetRb) return;

        // 1) Prioridad vertical: permitir colisionar si ya está "arriba" (aterriza)
        float platformTopY  = wallCollider.bounds.max.y;
        float playerBottomY = targetCollider.bounds.min.y;

        if (playerBottomY >= platformTopY - 0.05f)
        {
            Physics2D.IgnoreCollision(wallCollider, targetCollider, false);
            return;
        }

        // 2) Horizontal: permitir atravesar si se mueve hacia el muro
#if UNITY_6000_0_OR_NEWER
        float vx = targetRb.linearVelocity.x;
#else
        float vx = targetRb.velocity.x;
#endif
        bool shouldIgnore =
            (targetCollider.transform.position.x < transform.position.x && vx >  0.1f) ||
            (targetCollider.transform.position.x > transform.position.x && vx < -0.1f);

        Physics2D.IgnoreCollision(wallCollider, targetCollider, shouldIgnore);
    }

    // -------------------- Lógica Enemigos (solo horizontal) --------------------
    void ProcessEnemy(Collider2D targetCollider, Rigidbody2D targetRb)
    {
        if (!wallCollider || !targetCollider || !targetRb) return;

#if UNITY_6000_0_OR_NEWER
        float vx = targetRb.linearVelocity.x;
#else
        float vx = targetRb.velocity.x;
#endif
        bool shouldIgnore =
            (targetCollider.transform.position.x < transform.position.x && vx > 0f) ||
            (targetCollider.transform.position.x > transform.position.x && vx < 0f);

        Physics2D.IgnoreCollision(wallCollider, targetCollider, shouldIgnore);
    }
}
