using UnityEngine;

namespace RobotGame.States
{
    public abstract class StateMachine : MonoBehaviour
    {
        protected State State;

        public virtual void SetState(State state)
        {
            State = state;
            StartCoroutine(State.Start());
        }
        public virtual void TransitionState(State state)
        {
            StartCoroutine(State.End());
            State = state;
            StartCoroutine(State.Start());
        }
    }
}
