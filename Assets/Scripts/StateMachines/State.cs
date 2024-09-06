using System.Collections;
using UnityEngine;

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

        public virtual IEnumerator OnCollisionEnter2D(Collision2D collision)
        {
            yield break;
        }

        public virtual IEnumerator End()
        {
            yield break;
        }
    }
}