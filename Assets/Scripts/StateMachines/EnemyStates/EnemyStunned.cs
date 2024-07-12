using System.Collections;
using UnityEngine;

namespace RobotGame.States
{
    public class EnemyStunned : EnemyState
    {
        readonly GameObject enemy;
        public EnemyStunned(GameObject gameObject) { enemy = gameObject; name = "EnemyStunned"; }
        public override IEnumerator Start()
        {
            yield break;
        }

        public override IEnumerator Update()
        {
            enemy.GetComponent<SpriteRenderer>().color = Color.red;
            yield break;
        }
        public override IEnumerator End()
        {
            enemy.GetComponent<SpriteRenderer>().color = Color.black;
            yield break;
        }
    }
}