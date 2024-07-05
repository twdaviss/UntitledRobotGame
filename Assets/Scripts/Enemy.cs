using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RobotGame.States;
public class Enemy : EnemyStateMachine
{
    // Start is called before the first frame update
    void Start()
    {
        SetState(new EnemyIdle(this.gameObject));
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && State.name == "EnemyIdle")
        {
            TransitionState(new EnemyStunned(this.gameObject));
        }
        else if (Input.GetKeyDown(KeyCode.Space) && State.name == "EnemyStunned")
        {
            TransitionState(new EnemyIdle(this.gameObject));
        }
        StartCoroutine(State.Update());
    }
}
