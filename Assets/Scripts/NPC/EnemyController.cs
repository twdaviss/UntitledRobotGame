using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RobotGame.States;
public class EnemyController : EnemyStateMachine
{
    [SerializeField] public int moveSpeed;
    [SerializeField] public Transform targetLocation;

    private Rigidbody2D enemyRigidbody;
    private EnemyHealth enemyHealth;
    private Vector3 m_targetLocation;
    private List<Vector3> path;

    public float invincibilityTime = 0.0f;

    private void Awake()
    {
        enemyRigidbody = GetComponent<Rigidbody2D>();   
        enemyHealth = GetComponentInChildren<EnemyHealth>();
        m_targetLocation = targetLocation.position;
        SetState(new EnemyFollow(this));
    }
    
    void Update()
    {
        if(invincibilityTime > 0.0f) { invincibilityTime -= Time.deltaTime; }
        StartCoroutine(State.Update());
    }

    private void FixedUpdate()
    {
        StartCoroutine(State.FixedUpdate());
    }

    public void KnockBack(float knockBack, float damage, Vector2 direction)
    {
        if (invincibilityTime <= 0.0f)
        {
            enemyHealth.DealDamage(damage);
            enemyRigidbody.AddForce(knockBack * direction);
            TransitionState(new EnemyStaggered(this));
            invincibilityTime = 0.1f;
        }
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
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerProjectiles") && collision.enabled)
        {
            enemyHealth.DealDamage(collision.GetComponentInParent<Scrap>().GetDamage());
            collision.GetComponentInParent<Scrap>().ClampVelocity();
            collision.enabled = false;
        }
    }

    #region Pathfinding
    public List<Vector3> GetActivePath()
    {
        if(path == null)
        {
            GetNewPath();
        }
        return path;
    }
    public void GetNewPath()
    {
        if (path == null)
        {
            path = new List<Vector3>();
        }
        if(path.Count > 0)
        {
            path = Pathfinding.Instance.FindPath(path[0], m_targetLocation);
            return;
        }
        else
        {
            path = Pathfinding.Instance.FindPath(this.transform.position, m_targetLocation);
            return;
        }
    }
    public void CheckForTarget()
    {
        if (path == null || TargetMoved())
        {
            GetNewPath();
        }
        return;
    }
    public bool TargetMoved()
    {
        if (m_targetLocation != targetLocation.position)
        {
            m_targetLocation = targetLocation.position;
            return true;
        }
        return false;
    }
    
    public void ClearPath()
    {
        path = null;
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
