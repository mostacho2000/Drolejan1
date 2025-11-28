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

    [Header("Patrulla: detección de bordes/pared")]
    public Transform groundCheck;
    public Transform wallCheck;
    public float checkDistance = 0.2f;
    public LayerMask groundMask;

    [Header("Detección de jugador")]
    public float aggroDistance = 6f;   // Distancia para empezar a perseguir
    public float stopDistance = 2f;    // Distancia a la que deja de moverse y solo dispara

    bool facingRight = true;
    bool dead = false;

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
    }

    void Update()
    {
        if (dead) return;

        if (anim) anim.SetBool("Shoot", false);

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
                // 🔹 PERSEGUIR AL PLAYER
                MoveTowards(player.position);

                if (shootWhileRunning && withinFire)
                    shooter.TryShoot(player, anim);
            }
            else
            {
                // 🔹 YA ESTÁ CERCA → SE DETIENE Y DISPARA
                StopX();

                if (withinFire)
                    shooter.TryShoot(player, anim);
            }
        }
        else
        {
            // 🔹 Fuera de rango → patrulla
            MovePatrol();
        }

        if (anim)
            anim.SetFloat("Speed", Mathf.Abs(rb ? rb.linearVelocity.x : 0f));  // 👉 linearVelocity
    }

    //================== Movimiento ==================

    void MoveTowards(Vector3 target)
    {
        if (!rb) return;

        float dir = Mathf.Sign(target.x - transform.position.x);
        rb.linearVelocity = new Vector2(dir * speed, rb.linearVelocity.y);      // 👉 linearVelocity
    }

    void StopX()
    {
        if (rb)
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);           // 👉 linearVelocity
    }

    void MovePatrol()
    {
        if (!patrol)
        {
            StopX();
            return;
        }

        if (!rb)
            return;

        float dir = facingRight ? 1f : -1f;
        rb.linearVelocity = new Vector2(dir * speed, rb.linearVelocity.y);      // 👉 linearVelocity

        bool noGround = groundCheck &&
                        !Physics2D.Raycast(groundCheck.position, Vector2.down, checkDistance, groundMask);

        bool hitWall = wallCheck &&
                       Physics2D.Raycast(wallCheck.position,
                                         facingRight ? Vector2.right : Vector2.left,
                                         checkDistance,
                                         groundMask);

        if (noGround || hitWall)
            Flip();
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
        if (rb) rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);       // 👉 linearVelocity
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

        // Chequeos de suelo / pared
        Gizmos.color = Color.cyan;
        if (groundCheck)
            Gizmos.DrawLine(groundCheck.position,
                            groundCheck.position + Vector3.down * checkDistance);
        if (wallCheck)
            Gizmos.DrawLine(wallCheck.position,
                            wallCheck.position + (facingRight ? Vector3.right : Vector3.left) * checkDistance);
    }
}
