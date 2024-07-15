using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class Scrap : MonoBehaviour
{
    private Rigidbody2D scrapRigidbody;
    private ObjectPool<Scrap> scrapPool;
    private float moveSpeed;
    private float damage;
    private Vector2 direction;
    private float range;
    private float lifeTime;

    private void Awake()
    {
        scrapRigidbody = GetComponent<Rigidbody2D>();
    }
    private void OnEnable()
    {
        lifeTime = range;
        scrapRigidbody.AddForce(direction * moveSpeed, ForceMode2D.Force);
    }
    void Update()
    {
        lifeTime -= Time.deltaTime;
        if(lifeTime <= 0)
        {
            scrapPool.Release(this);
        }
    }

    public void SetParameters(float moveSpeed, float damage, float range, Vector2 direction)
    {
        this.moveSpeed = moveSpeed;
        this.damage = damage;
        this.range = range;
        this.direction = direction;
    }

    public void SetPool(ObjectPool<Scrap> pool)
    {
        scrapPool = pool;
    }

    public void ClampVelocity()
    {
        scrapRigidbody.velocity = Vector3.ClampMagnitude(scrapRigidbody.velocity, 5);
    }

    public float GetDamage()
    {
        return damage;
    }
}
