using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RobotGame.States;
using RobotGame;
public class Enemy : EnemyStateMachine
{
    [SerializeField] public int moveSpeed;
    [SerializeField] public Transform targetLocation;
    private Transform m_targetLocation;

    private List<Vector3> path;
    [HideInInspector] public int CurrentPathIndex { get; set; }

    private void Awake()
    {
        if (m_targetLocation == null)
        {
            m_targetLocation = targetLocation;
        }
        
        SetState(new EnemyIdle(this));
    }
    
    void Update()
    {
        if (targetLocation != m_targetLocation)
        {
            GetPath();
        }
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
        path = Pathfinding.Instance.FindPath(this.transform.position, m_targetLocation.position);
        CurrentPathIndex = 0;
    }
    
    public void ClearPath()
    {
        path = null;
    }
}
