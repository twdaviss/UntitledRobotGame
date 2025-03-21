using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RobotGame.States;
using System;
using static UnityEngine.EventSystems.EventTrigger;

public enum EnemyType
{
    Aggressive,
    Shy,
    Ranged,
    Explosive,
}

public class EnemyController : EnemyStateMachine
{
    [HideInInspector] public Animator enemyAnimator;

    [Header("General")]
    [SerializeField] public EnemyType enemyType;
    [SerializeField] private GameObject projectile;
    [SerializeField] public int moveSpeed;
    [SerializeField] public Transform target;

    [Header("Melee")]
    [SerializeField] public float meleeDamage;
    [SerializeField] public float meleeRange;
    [SerializeField] public float meleeBuildUpTime;
    [SerializeField] public float meleeRecoveryTime;
    [SerializeField] public float meleeDamageTime;
    [SerializeField] public float meleeCooldown;
    [SerializeField] public AudioClip zapSound;

    [Header("Ranged")]
    [SerializeField] public float shootBuildUpTime;
    [SerializeField] public float shootRecoveryTime;
    [SerializeField] public float shootTime;
    [SerializeField] public float shootCooldown;
    [SerializeField] public float shootRange;
    [SerializeField] public float projectileSpeed;

    [Header("Wander")]
    [SerializeField] public float wanderWaitTime;
    [SerializeField] public float wanderTime;
    [SerializeField] public int wanderMultiplier;

    [Header("Flee")]
    [SerializeField] public float fleeDistanceThreshold;
    [SerializeField] public float fleeTime;

    private ParticleSystem enemyParticleSystem;
    private AudioSource enemyAudioSource;
    private Rigidbody2D enemyRigidbody;
    private EnemyHealth enemyHealth;
    private EnemyStun enemyStun;
    private Vector3 prevTargetPosition;
    private Vector3 destination;
    private List<Vector3> path;
    private float invincibilityTime = 0.0f;

    private void Awake()
    {
        enemyRigidbody = GetComponent<Rigidbody2D>();
        enemyHealth = GetComponentInChildren<EnemyHealth>();
        enemyStun = GetComponentInChildren<EnemyStun>();
        enemyParticleSystem = GetComponentInChildren<ParticleSystem>();
        enemyAnimator = GetComponent<Animator>();
        enemyAudioSource = GetComponent<AudioSource>();
        destination = target.position;
        prevTargetPosition = target.position;
        path = null;

        SetState(new EnemyIdle(this));
    }

    void Update()
    {
        if (invincibilityTime > 0.0f) { invincibilityTime -= Time.deltaTime; }

        if (!GameManager.Instance.IsPauseMenuEnabled())
        {
            StartCoroutine(State.Update());
        }
    }

    private void FixedUpdate()
    {
        StartCoroutine(State.FixedUpdate());
    }

    public void Damage(float damage = 0, float stun = 0, float knockBack = 0, Vector2 direction = default)
    {
        if (invincibilityTime > 0.0f) { return; }

        if (damage > 0)
        {
            if (State.GetType() == typeof(EnemyStaggered))
            {
                enemyHealth.DealDamage(2 * damage);
            }
            else
            {
                enemyHealth.DealDamage(damage);
            }
        }

        if (stun > 0) { enemyStun.DealDamage(stun); }

        if (knockBack > 0)
        {
            TransitionState(new EnemyKnockback(this, knockBack, direction));
            GameManager.Instance.FreezeTimeScale(0.1f);
        }

        invincibilityTime = 0.1f;
    }

    public void Shoot(Vector2 direction, float speed)
    {
        GameObject enemyProjectile = Instantiate(projectile,transform.position, Quaternion.identity);
        enemyProjectile.GetComponent<Rigidbody2D>().AddForce(direction * speed, ForceMode2D.Force);
        enemyProjectile.transform.rotation = Quaternion.Euler(-90,0,0);
    }

    public void PlayZapSound()
    {
        enemyAudioSource.clip = zapSound;
        enemyAudioSource.pitch = 1.5f;
        enemyAudioSource.Play();
    }
    public bool CheckMeleeRange()
    {
        if (Vector3.Distance(target.transform.position, transform.position) < meleeRange)
        {
            return true;
        }
        return false;
    }
    public bool CheckShootRange()
    {
        if (Vector3.Distance(target.transform.position, transform.position) < shootRange)
        {
            return true;
        }
        return false;
    }
    public IEnumerator Despawn()
    {
        Destroy(gameObject);
        yield return new WaitForSeconds(1.0f);
    }

    public void StopMoving()
    {
        enemyRigidbody.velocity = Vector3.zero;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("PlayerProjectiles"))
        {
            if (collision.gameObject.GetComponent<Scrap>().inert)
            {
                return;
            }
            Scrap scrap = collision.gameObject.GetComponent<Scrap>();
            Damage(scrap.GetDamage(), scrap.GetStun());
            scrap.ClampVelocity();
        }
    }

    #region Pathfinding
    public void MoveToNextPoint()
    {
        Vector2 moveTarget = path[0];
        if (Vector3.Distance(transform.position, moveTarget) < 1.0)
        {
            path.RemoveAt(0);

            if (path.Count <= 0)
            {
                StopMoving();
                Debug.Log("Reached target");
            }
        }

        Vector2 moveDirection = (moveTarget - (Vector2)transform.position).normalized;
        transform.position += (Vector3)(moveDirection * (moveSpeed * Time.deltaTime));
    }

    public List<Vector3> GetActivePath()
    {
        return path;
    }

    public void GetPathToTarget()
    {
        destination = target.position;
        if (path != null)
        {
            if (path.Count > 1)
            {
                path = Pathfinding.Instance.FindPath(path[0], destination);
                return;
            }
        }
        path = Pathfinding.Instance.FindPath(this.transform.position, destination);
    }

    public void GetPathToDestination(Vector2 dest)
    {
        destination = dest;
        path = Pathfinding.Instance.FindPath(this.transform.position, destination);
    }

    public bool CheckTarget()
    {
        if (TargetMoved() || path == null || path.Count == 0)
        {
            GetPathToTarget();
        }
        if(path == null)
        {
            return false;
        }
        return true;
    }

    public bool CheckDestination(Vector2 dest)
    {
        GetPathToDestination(dest);
        if(path == null)
        {
            return false;
        }
        return true;
    }

    public bool TargetMoved()
    {
        if (Vector2.Distance(target.position, prevTargetPosition) > 1.5f)
        {
            prevTargetPosition = target.position;
            return true;
        }
        return false;
    }
    
    public void ClearPath()
    {
        path = null;
    }

    public void ReleaseSparks()
    {
        enemyParticleSystem.Play();
    }

    private void OnDrawGizmos()
    {
        if(path == null) { return; }
        if (path.Count <= 0) { return; }

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, path[0]);
        for (int i = 0; i < path.Count - 1; i++)
        {
            Gizmos.DrawLine(path[i], path[i + 1]);
        }
    }
    #endregion
}
