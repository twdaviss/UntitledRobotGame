using System.Collections;

namespace RobotGame.States
{
    public abstract class State
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

        public virtual IEnumerator End()
        {
            yield break;
        }
    }
}