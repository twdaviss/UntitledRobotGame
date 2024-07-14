using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class Scrap : MonoBehaviour
{
    private Rigidbody2D scrapRigidbody;
    private ObjectPool<Scrap> scrapPool;
    public float moveSpeed;
    public float damage;
    public Vector2 direction;
    public float range;

    private float lifeTime;

    private void Awake()
    {
        scrapRigidbody = GetComponent<Rigidbody2D>();
    }
    private void OnEnable()
    {
        lifeTime = range;
    }
    void Update()
    {
        scrapRigidbody.velocity = direction * moveSpeed;
        lifeTime -= Time.deltaTime;
        if(lifeTime <= 0)
        {
            scrapPool.Release(this);
        }
    }

    public void SetPool(ObjectPool<Scrap> pool)
    {
        scrapPool = pool;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Enemy"))
        {
            scrapPool.Release(this);
        }
    }
}
