using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RobotGame.States;
using RobotGame;
public class Enemy : EnemyStateMachine
{
    [SerializeField] public int moveSpeed;
    [SerializeField] public Transform targetLocation;
    private Vector3 m_targetLocation;

    private List<Vector3> path;

    private void Awake()
    {
        m_targetLocation = targetLocation.position;
        
        SetState(new EnemyIdle(this));
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && State.name == "EnemyIdle")
        {
            TransitionState(new EnemyStunned(this.gameObject));
        }
        else if (Input.GetKeyDown(KeyCode.Space) && State.name == "EnemyStunned")
        {
            TransitionState(new EnemyIdle(this));
        }
        StartCoroutine(State.Update());
    }

    public List<Vector3> GetActivePath()
    {
        if(path == null)
        {
            GetPath();
        }
        return path;
    }
    public void GetPath()
    {
        if (path == null)
        {
            path = new List<Vector3>();
        }
        if(path.Count > 0)
        {
            path = Pathfinding.Instance.FindPath(path[0], m_targetLocation);
        }
        else
        {
            path = Pathfinding.Instance.FindPath(this.transform.position, m_targetLocation);
        }
        Debug.Log("Target path calculated");
        return;
    }
    public void CheckForTarget()
    {
        if (path == null || TargetMoved())
        {
            GetPath();
        }
        return;
    }
    public bool TargetMoved()
    {
        if (m_targetLocation != targetLocation.position)
        {
            m_targetLocation = targetLocation.position;
            Debug.Log("Target location changed");
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
        for (int i = 0; i < path.Count - 1; i++)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(path[i], path[i + 1]);
        }
    }

}
