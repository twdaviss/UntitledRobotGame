using UnityEngine;

namespace RobotGame.States
{
    public abstract class PlayerStateMachine : MonoBehaviour
    {
        protected PlayerState State;

        public virtual void SetState(PlayerState state)
        {
            State = state;
            StartCoroutine(State.Start());
        }
        public virtual void TransitionState(PlayerState state)
        {
            StartCoroutine(State.End());
            State = state;
            StartCoroutine(State.Start());
        }
    }

    public class EnemyStateMachine : MonoBehaviour
    {
        protected EnemyState State;

        public virtual void SetState(EnemyState state)
        {
            State = state;
            StartCoroutine(State.Start());
        }
        public virtual void TransitionState(EnemyState state)
        {
            StartCoroutine(State.End());
            State = state;
            StartCoroutine(State.Start());
        }
    }
}