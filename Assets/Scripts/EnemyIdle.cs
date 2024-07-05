using System.Collections;
using UnityEngine;

namespace RobotGame.States
{
    public class EnemyIdle : EnemyState
    {
        readonly GameObject enemy;
        public EnemyIdle(GameObject gameObject) { enemy = gameObject; name = "EnemyIdle"; }
        public override IEnumerator Start()
        {
            yield break;
        }

        public override IEnumerator Update()
        {
            enemy.transform.Rotate(0, 0, 1);
            yield break;
        }
    }
}