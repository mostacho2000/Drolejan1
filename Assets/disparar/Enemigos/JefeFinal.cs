using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D), typeof(Animator))]
public class JefeFinal : MonoBehaviour, IDamageable
{
    [Header("Vida")]
    [SerializeField] int maxHP = 10;
    [SerializeField] int puntosAlMorir = 3;
    [SerializeField] string winScene = ""; // "Escena Win Ricardo" o vacío para no cambiar escena
    int hp;
    bool dead;

    [Header("Movimiento / IA simple")]
    [SerializeField] float speed = 5f;
    [SerializeField] float aggroDistance = 15f;
    [SerializeField] float stopDistance = 1f;
    [SerializeField] bool shootWhileRunning = false;

    [Header("Disparo")]
    [SerializeField] Transform shootOrigin;
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] float bulletSpeed = 10f;
    [SerializeField] float fireRate = 1.0f;
    [SerializeField] float fireRange = 10f;

    [Header("Referencias")]
    [SerializeField] Animator anim;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] SpriteRenderer sr;

    Transform player;
    float fireTimer;
    bool wantShoot;

    static readonly int HASH_SPEED = Animator.StringToHash("Speed");
    static readonly int HASH_SHOOT = Animator.StringToHash("Shoot");
    static readonly int HASH_DIE   = Animator.StringToHash("Die");

    void Reset()
    {
        rb   = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sr   = GetComponent<SpriteRenderer>();

#if UNITY_6000_0_OR_NEWER
        rb.bodyType = RigidbodyType2D.Kinematic;
#else
        rb.isKinematic = true;
#endif
        rb.freezeRotation = true;
    }

    void Awake()
    {
        hp = Mathf.Max(1, maxHP);
        if (!rb)   rb = GetComponent<Rigidbody2D>();
        if (!anim) anim = GetComponent<Animator>();
        if (!sr)   sr = GetComponentInChildren<SpriteRenderer>();
    }

    void Start()
    {
        var p1 = GameObject.FindGameObjectWithTag("Player");
        var p2 = GameObject.FindGameObjectWithTag("Player1");
        player = (p1 ? p1.transform : null) ?? (p2 ? p2.transform : null);

        if (!shootOrigin) shootOrigin = transform;
    }

    void Update()
    {
        if (dead) return;

        Vector2 vel = Vector2.zero;

        if (player)
        {
            Vector2 toPlayer = player.position - transform.position;
            float dist = toPlayer.magnitude;
            int dir = toPlayer.x >= 0 ? 1 : -1;

            // Voltear sprite del jefe
            if (sr) sr.flipX = (dir < 0);
            else
            {
                var sc = transform.localScale;
                sc.x = Mathf.Abs(sc.x) * dir;
                transform.localScale = sc;
            }

            // ***** IMPORTANTE: mover el punto de disparo al lado correcto *****
            if (shootOrigin)
            {
                Vector3 local = shootOrigin.localPosition;
                float absX = Mathf.Abs(local.x);
                local.x = absX * (dir >= 0 ? 1f : -1f);   // derecha +, izquierda -
                shootOrigin.localPosition = local;
            }

            // Lógica de rango / movimiento / disparo
            fireTimer += Time.deltaTime;
            bool inFireRange = dist <= fireRange;
            bool canShoot    = inFireRange && fireTimer >= fireRate;

            bool shouldMove = dist > stopDistance || (shootWhileRunning && !inFireRange);
            if (shouldMove && dist <= aggroDistance)
                vel = new Vector2(dir * speed, 0f);

            wantShoot = inFireRange;
            if (!shootWhileRunning && inFireRange)
                vel = Vector2.zero;

            if (canShoot && wantShoot)
            {
                fireTimer = 0f;
                anim.SetBool(HASH_SHOOT, true); // la bala se instancia por Animation Event
            }
        }

#if UNITY_6000_0_OR_NEWER
        if (rb && rb.bodyType == RigidbodyType2D.Kinematic)
            rb.MovePosition(rb.position + vel * Time.deltaTime);
        else
            rb.linearVelocity = new Vector2(vel.x, rb.linearVelocity.y);
#else
        if (rb && rb.isKinematic)
            rb.MovePosition(rb.position + vel * Time.deltaTime);
        else
            rb.velocity = new Vector2(vel.x, rb.velocity.y);
#endif

        if (anim)
        {
            anim.SetFloat(HASH_SPEED, Mathf.Abs(vel.x));
            if (!wantShoot)
                anim.SetBool(HASH_SHOOT, false);
        }
    }

    // Llamado desde el frame del destello en la animación de disparo
    public void AnimEvent_Fire()
    {
        if (dead) return;
        if (!bulletPrefab || !shootOrigin) return;

        int dirX = (sr && sr.flipX) ? -1 : 1;

        GameObject go = Instantiate(bulletPrefab, shootOrigin.position, Quaternion.identity);
        EnemyBullet2D bullet = go.GetComponent<EnemyBullet2D>();
        if (bullet != null)
        {
            bullet.Setup(new Vector2(dirX, 0f), bulletSpeed);
        }
    }

    // ================== DAÑO (llamado por PlayerBullet) ==================
    public void TakeDamage(int amount, Vector2 hitPoint, Vector2 hitDir)
    {
        if (dead) return;

        hp -= Mathf.Max(1, amount);

        if (hp <= 0)
            Die();
    }

    // Overload por si en algún lugar solo llaman (int)
    public void TakeDamage(int amount)
    {
        TakeDamage(amount, transform.position, Vector2.zero);
    }

    void Die()
    {
        if (dead) return;
        dead = true;

#if UNITY_6000_0_OR_NEWER
        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;
#else
        rb.velocity = Vector2.zero;
        rb.isKinematic = true;
#endif

        Collider2D col = GetComponent<Collider2D>();
        if (col) col.enabled = false;

        GameManager.instancia?.CambiarPuntos(puntosAlMorir);

        if (anim)
            anim.SetTrigger(HASH_DIE);

        Invoke(nameof(FinishDeath), 1.2f);
    }

    public void AnimEvent_DeathEnd()
    {
        FinishDeath();
    }

    void FinishDeath()
    {
        if (!string.IsNullOrEmpty(winScene))
            SceneManager.LoadScene(winScene);
        else
            Destroy(gameObject);
    }
}
