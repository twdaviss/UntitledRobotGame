using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RobotGame.States;
using RobotGame;
public class EnemyController : EnemyStateMachine
{
    [SerializeField] public int moveSpeed;
    [SerializeField] public Transform targetLocation;
   
    private Vector3 m_targetLocation;
    private List<Vector3> path;

    private void Awake()
    {
        m_targetLocation = targetLocation.position;
        SetState(new EnemyFollow(this));
    }
    
    void Update()
    {
        StartCoroutine(State.Update());
    }

    private void FixedUpdate()
    {
        StartCoroutine(State.FixedUpdate());
    }

    public IEnumerator Despawn()
    {
        Destroy(gameObject);
        yield return new WaitForSeconds(1.0f);
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
