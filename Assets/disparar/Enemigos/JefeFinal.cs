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
    [SerializeField] float speed = 2.2f;
    [SerializeField] float aggroDistance = 12f;
    [SerializeField] float stopDistance = 4.5f;
    [SerializeField] bool shootWhileRunning = false;

    [Header("Disparo")]
    [SerializeField] Transform shootOrigin;
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] float bulletSpeed = 12f;
    [SerializeField] float fireRate = 1.0f;
    [SerializeField] float fireRange = 9f;

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
        rb  = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sr  = GetComponent<SpriteRenderer>();

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

            if (sr) sr.flipX = (dir < 0);
            else transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * dir, transform.localScale.y, transform.localScale.z);

            fireTimer += Time.deltaTime;
            bool inFireRange = dist <= fireRange;
            bool canShoot = inFireRange && fireTimer >= fireRate;

            bool shouldMove = dist > stopDistance || (shootWhileRunning && !inFireRange);
            if (shouldMove && dist <= aggroDistance)
                vel = new Vector2(dir * speed, 0f);

            wantShoot = inFireRange;
            if (!shootWhileRunning && inFireRange)
                vel = Vector2.zero;

            if (canShoot && wantShoot)
            {
                fireTimer = 0f;
                anim.SetBool(HASH_SHOOT, true);   // la bala se crea con el Animation Event
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

        anim.SetFloat(HASH_SPEED, Mathf.Abs(vel.x));

        if (!wantShoot)
            anim.SetBool(HASH_SHOOT, false);
    }

    // Llamado desde el frame del destello en la animación de disparo
    public void AnimEvent_Fire()
    {
        if (dead) return;
        if (!bulletPrefab || !shootOrigin) return;

        int dir = 1;
        if (sr) dir = sr.flipX ? -1 : 1;
        else    dir = transform.localScale.x >= 0 ? 1 : -1;

        var b = Instantiate(bulletPrefab, shootOrigin.position, Quaternion.identity);
        var rbB = b.GetComponent<Rigidbody2D>();
#if UNITY_6000_0_OR_NEWER
        if (rbB) rbB.linearVelocity = new Vector2(dir * bulletSpeed, 0f);
#else
        if (rbB) rbB.velocity = new Vector2(dir * bulletSpeed, 0f);
#endif
    }

    // Sistema nuevo (IDamageable)
    public void TakeDamage(int amount, Vector2 hitPoint, Vector2 hitDir)
    {
        if (dead) return;
        hp -= Mathf.Max(1, amount);
        if (hp <= 0) Die();
    }

    // Compatibilidad con tags antiguos y colliders Trigger
    void OnTriggerEnter2D(Collider2D other)
    {
        if (dead) return;

        if (other.CompareTag("balaBuena"))
        {
            hp -= 1;
            Destroy(other.gameObject);
            if (hp <= 0) Die();
            return;
        }
        if (other.CompareTag("granada"))
        {
            hp -= 3;
            if (hp <= 0) Die();
        }
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
        var col = GetComponent<Collider2D>();
        if (col) col.enabled = false;

        GameManager.instancia?.CambiarPuntos(puntosAlMorir);

        anim.SetTrigger(HASH_DIE);
        Invoke(nameof(FinishDeath), 1.2f);
    }

    public void AnimEvent_DeathEnd() => FinishDeath();

    void FinishDeath()
    {
        if (!string.IsNullOrEmpty(winScene))
            SceneManager.LoadScene(winScene);
        else
            Destroy(gameObject);
    }
}
