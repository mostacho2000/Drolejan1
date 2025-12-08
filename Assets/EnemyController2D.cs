using UnityEngine;

public class EnemyController2D : MonoBehaviour
{
    [Header("Referencias")]
    public Transform player;             // Se auto-asigna por Tag "Player" si está vacío
    public Enemi_shot shooter;           // Script de disparo del enemigo
    public Animator anim;
    public Rigidbody2D rb;
    public SpriteRenderer sr;

    [Header("Movimiento")]
    public float speed = 2.5f;
    public bool patrol = true;
    public bool shootWhileRunning = false;

    [Header("Patrulla SIMPLE (izquierda - derecha)")]
    [Tooltip("Distancia total que recorre desde el centro hacia cada lado")]
    public float patrolDistance = 3f;

    [Header("Detección de jugador")]
    public float aggroDistance = 6f;   // Distancia para empezar a perseguir
    public float stopDistance = 2f;    // Distancia a la que deja de moverse y solo dispara

    bool facingRight = true;
    bool dead = false;

    // límites de la patrulla
    float startX;
    float leftLimitX;
    float rightLimitX;

    void Reset()
    {
        anim = GetComponent<Animator>();
        rb   = GetComponent<Rigidbody2D>();
        sr   = GetComponent<SpriteRenderer>();
        shooter = GetComponent<Enemi_shot>();
    }

    void Awake()
    {
        if (!player)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p) player = p.transform;
        }

        if (!rb)      rb      = GetComponent<Rigidbody2D>();
        if (!anim)    anim    = GetComponent<Animator>();
        if (!sr)      sr      = GetComponent<SpriteRenderer>();
        if (!shooter) shooter = GetComponent<Enemi_shot>();

        // ---- Configurar limites de patrulla simple ----
        startX     = transform.position.x;
        leftLimitX = startX - patrolDistance;
        rightLimitX = startX + patrolDistance;
    }

    void Update()
    {
        if (dead) return;

        if (anim) anim.SetBool("Shoot", false);

        // Si no hay player, solo patrulla
        if (!player)
        {
            MovePatrol();
            return;
        }

        float dist = Vector2.Distance(transform.position, player.position);
        bool canSee = shooter ? shooter.HasLOS(player) : true;

        // Siempre mirar hacia el player si está en rango de aggro
        if (dist <= aggroDistance)
            LookAt(player.position);

        if (dist <= aggroDistance && canSee)
        {
            bool withinStop = dist <= stopDistance;
            bool withinFire = shooter ? dist <= shooter.fireRange : true;

            if (!withinStop)
            {
                // PERSEGUIR AL PLAYER
                MoveTowards(player.position);

                if (shootWhileRunning && withinFire)
                    shooter.TryShoot(player, anim);
            }
            else
            {
                // YA ESTÁ CERCA → SE DETIENE Y DISPARA
                StopX();

                if (withinFire)
                    shooter.TryShoot(player, anim);
            }
        }
        else
        {
            // Fuera de rango → patrulla simple
            MovePatrol();
        }

        if (anim)
            anim.SetFloat("Speed", Mathf.Abs(rb ? rb.linearVelocity.x : 0f));
    }

    // ================== Movimiento ==================

    void MoveTowards(Vector3 target)
    {
        if (!rb) return;

        float dir = Mathf.Sign(target.x - transform.position.x);
        rb.linearVelocity = new Vector2(dir * speed, rb.linearVelocity.y);
    }

    void StopX()
    {
        if (rb)
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
    }

    // PATRULLA SIMPLE: va de leftLimitX a rightLimitX
    void MovePatrol()
    {
        if (!patrol || !rb) return;

        float dir = facingRight ? 1f : -1f;
        rb.linearVelocity = new Vector2(dir * speed, rb.linearVelocity.y);

        float x = transform.position.x;

        // Si va a la derecha y ya pasó el límite derecho → voltear
        if (facingRight && x >= rightLimitX)
        {
            Flip();
        }
        // Si va a la izquierda y ya pasó el límite izquierdo → voltear
        else if (!facingRight && x <= leftLimitX)
        {
            Flip();
        }
    }

    void LookAt(Vector3 target)
    {
        if ((target.x > transform.position.x) != facingRight)
            Flip();
    }

    void Flip()
    {
        facingRight = !facingRight;

        Vector3 s = transform.localScale;
        s.x *= -1f;
        transform.localScale = s;
    }

    public void OnDeath()
    {
        dead = true;
        StopX();
        if (rb) rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
    }

    void OnDrawGizmosSelected()
    {
        // Aggro
        Gizmos.color = new Color(1f, 1f, 0f, 0.4f);
        Gizmos.DrawWireSphere(transform.position, aggroDistance);

        // Rango de disparo
        if (shooter)
        {
            Gizmos.color = new Color(1f, 0f, 0f, 0.3f);
            Gizmos.DrawWireSphere(transform.position, shooter.fireRange);
        }

        // Límites de patrulla (solo en editor)
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(new Vector3(leftLimitX, transform.position.y - 0.5f, 0),
                        new Vector3(leftLimitX, transform.position.y + 0.5f, 0));
        Gizmos.DrawLine(new Vector3(rightLimitX, transform.position.y - 0.5f, 0),
                        new Vector3(rightLimitX, transform.position.y + 0.5f, 0));
    }
}
