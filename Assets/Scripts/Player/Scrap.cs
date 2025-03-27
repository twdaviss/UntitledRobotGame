using UnityEngine;
using UnityEngine.Pool;

public class Scrap : MonoBehaviour
{
    private Rigidbody2D scrapRigidbody;
    private ObjectPool<Scrap> scrapPool;
    private float moveSpeed;
    private float damage;
    private float stun;
    private Vector2 direction;
    private float range;
    private float lifeTime;

    private GameObject player;
    private GameObject prevEnemy;
    private bool isMagnetized = false;
    private float absorbDelay = 0.1f;
    private float absorbTime = 0.0f;
    private int bounces = 2;

    [HideInInspector] public bool canRicochet = false;
    [HideInInspector] public bool inert = false;
    private void Awake()
    {
        scrapRigidbody = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        InputManager.onMagnetizeScrap += Magnetize;
        lifeTime = range;
        bounces = 2;
        scrapRigidbody.AddForce(direction * moveSpeed, ForceMode2D.Force);
        isMagnetized = false;
    }

    private void OnDisable()
    {
        InputManager.onMagnetizeScrap -= Magnetize;
    }

    private void OnDestroy()
    {
        InputManager.onMagnetizeScrap -= Magnetize;
    }

    void Update()
    {
        if(scrapRigidbody.velocity.magnitude > 2.0f)
        {
            transform.Rotate(0, 360 * Time.deltaTime, 360 * Time.deltaTime, Space.World);
        }
        if(isMagnetized)
        {
            Vector3 spawnPosition = player.transform.position;
            spawnPosition.z -= 1;
            Vector3 direction = (spawnPosition - transform.position).normalized;
            transform.position += direction * 40f * Time.deltaTime;
            inert = false;
        }
        if (absorbTime < absorbDelay) { absorbTime += Time.deltaTime;}
        lifeTime -= Time.deltaTime;
        if(lifeTime <= 0)
        {
            scrapPool.Release(this);
        }
    }

    public void SetParameters(float moveSpeed, float damage, float stun, float range, Vector2 direction, GameObject player)
    {
        this.moveSpeed = moveSpeed;
        this.damage = damage;
        this.stun = stun;
        this.range = range;
        this.direction = direction;
        this.player = player;
        this.inert = false;
    }

    public void SetPool(ObjectPool<Scrap> pool)
    {
        scrapPool = pool;
    }

    public void ClampVelocity()
    {
        scrapRigidbody.velocity = Vector3.ClampMagnitude(scrapRigidbody.velocity, 5);
        inert = true;
    }

    public float GetDamage()
    {
        return damage;
    }

    public float GetStun()
    {
        return stun;
    }

    public void Magnetize()
    {
        isMagnetized = true;
        scrapRigidbody.velocity = Vector2.zero;
    }

    public void Ricochet()
    {
        if(bounces <= 0)
        {
            ClampVelocity();
            return;
        }
        inert = false;
        isMagnetized = false;
        int layerMask = LayerMask.GetMask("Enemies");
        Vector2 position = Vector2.zero;
        Collider2D[] targets = Physics2D.OverlapCircleAll(player.transform.position, 15, layerMask);
        foreach (Collider2D target in targets)
        {
            if (target.gameObject.GetComponent<EnemyController>() == null || target.gameObject == prevEnemy)
            {
                continue;
            }

            float distance = Vector2.Distance(target.transform.position, transform.position);
            if (distance < 15 && distance >= 4)
            {
                direction = (target.transform.position - transform.position).normalized;
                scrapRigidbody.AddForce(direction * moveSpeed, ForceMode2D.Force);
                bounces--;
                return;
            }
        }
        bounces = 0;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(absorbTime < absorbDelay)
        {
            return;
        }
        if(collision.CompareTag("Player"))
        {
            isMagnetized = false;
            absorbTime = 0;
            scrapPool.Release(this);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            if (canRicochet)
            {
                prevEnemy = collision.gameObject;
                Ricochet();
            }
        }
    }

}
