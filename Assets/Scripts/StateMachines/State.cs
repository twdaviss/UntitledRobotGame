using System.Collections;

namespace RobotGame.States
{
    public class EnemyState
    {
        public string name;
        public virtual IEnumerator Start()
        {
            yield break;
        }

        public virtual IEnumerator Update()
        {
            yield break;
        }

        public virtual IEnumerator FixedUpdate()
        {
            yield break;
        }

        public virtual IEnumerator End()
        {
            yield break;
        }
    }

    public class PlayerState
    {
        public string name;
        public virtual IEnumerator Start()
        {
            yield break;
        }

        public virtual IEnumerator Update()
        {
            yield break;
        }

        public virtual IEnumerator FixedUpdate()
        {
            yield break;
        }

        public virtual IEnumerator End()
        {
            yield break;
        }
    }
}