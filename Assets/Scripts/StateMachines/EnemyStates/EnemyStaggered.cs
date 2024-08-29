using System.Collections;
using UnityEngine;

namespace RobotGame.States
{
    public class EnemyStaggered : EnemyState
    {
        readonly private EnemyController enemy;
        readonly private float duration = 0.3f;
        private float currentTime = 0.0f;
        public EnemyStaggered(EnemyController enemy) { this.enemy = enemy; this.name = "EnemyStaggered"; }
        public override IEnumerator Start()
        {
            yield break;
        }

        public override IEnumerator Update()
        {
            if(currentTime < duration)
            {
                currentTime += Time.deltaTime;
            }
            else
            {
                enemy.TransitionState(new EnemyFollow(enemy));
                yield break;
            }
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