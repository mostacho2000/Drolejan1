using UnityEngine;

public class EnemyController2D : MonoBehaviour
{
    [Header("Referencias")]
    public Transform player;             // se auto-asigna por tag "Player" si está vacío
    public Enemi_shot shooter;           // script de disparo (mismo objeto)
    public Animator anim;                // parámetros: Speed (float), Shoot (bool), Die (trigger)
    public Rigidbody2D rb;               
    public SpriteRenderer sr;

    [Header("Movimiento")]
    public float speed = 2f;
    public bool patrol = true;
    public bool shootWhileRunning = false;

    [Header("Patrulla: detección de bordes/pared")]
    public Transform groundCheck;
    public Transform wallCheck;
    public float checkDistance = 0.2f;
    public LayerMask groundMask;

    [Header("Detección de jugador")]
    public float aggroDistance = 12f;    // distancia para empezar a interactuar
    public float stopDistance = 2f;      // no nos acercamos más que esto

    bool facingRight = true;
    bool dead = false;

    void Reset()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        shooter = GetComponent<Enemi_shot>();
    }

    void Awake()
    {
        if (!player)
        {
            var p = GameObject.FindGameObjectWithTag("Player");
            if (p) player = p.transform;
        }
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
        LookAt(player.position);

        bool canSee = shooter ? shooter.HasLOS(player) : true;

        if (dist <= aggroDistance && canSee)
        {
            if (dist > Mathf.Max(stopDistance, shooter ? shooter.fireRange : 0f))
            {
                // acercarse
                MoveTowards(player.position);
            }
            else
            {
                // detenerse y disparar
                if (!shootWhileRunning) StopX();
                shooter?.TryShoot(player, anim);
            }
        }
        else
        {
            MovePatrol();
        }

        if (anim) anim.SetFloat("Speed", Mathf.Abs(rb ? rb.linearVelocity.x : 0f));
    }

    void MoveTowards(Vector3 target)
    {
        float dir = Mathf.Sign(target.x - transform.position.x);
        if (rb) rb.linearVelocity = new Vector2(dir * speed, rb.linearVelocity.y);
    }

    void StopX()
    {
        if (rb) rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
    }

    void MovePatrol()
    {
        if (!patrol) { StopX(); return; }

        float dir = facingRight ? 1f : -1f;
        if (rb) rb.linearVelocity = new Vector2(dir * speed, rb.linearVelocity.y);

        bool noGround = groundCheck && !Physics2D.Raycast(groundCheck.position, Vector2.down, checkDistance, groundMask);
        bool hitWall  = wallCheck   &&  Physics2D.Raycast(wallCheck.position, new Vector2(dir, 0f), checkDistance, groundMask);

        if (noGround || hitWall) Flip();
    }

    void LookAt(Vector3 target)
    {
        if ((target.x > transform.position.x) != facingRight) Flip();
    }

    void Flip()
    {
        facingRight = !facingRight;
        var ls = transform.localScale; ls.x *= -1f; transform.localScale = ls;
    }

    public void OnDeath()
    {
        dead = true;
        StopX();
        if (rb) rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 1, 0, .5f);
        Gizmos.DrawWireSphere(transform.position, aggroDistance);
        if (shooter) { Gizmos.color = Color.red; Gizmos.DrawWireSphere(transform.position, shooter.fireRange); }
        Gizmos.color = Color.cyan;
        if (groundCheck) Gizmos.DrawLine(groundCheck.position, groundCheck.position + Vector3.down * checkDistance);
        if (wallCheck)   Gizmos.DrawLine(wallCheck.position,   wallCheck.position   + (facingRight ? Vector3.right : Vector3.left) * checkDistance);
    }
}
