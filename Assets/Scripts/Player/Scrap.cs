using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UIElements;

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
    private bool isMagnetized = false;
    private float absorbDelay = 0.1f;
    private float absorbTime = 0.0f;
    
    public bool inert = false;
    private void Awake()
    {
        scrapRigidbody = GetComponent<Rigidbody2D>();
    }
    private void OnEnable()
    {
        InputManager.onMagnetize += Magnetize;
        lifeTime = range;
        scrapRigidbody.AddForce(direction * moveSpeed, ForceMode2D.Force);
    }

    private void OnDestroy()
    {
        InputManager.onMagnetize -= Magnetize;
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

}
