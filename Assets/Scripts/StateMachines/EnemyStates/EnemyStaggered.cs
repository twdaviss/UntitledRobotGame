using System.Collections;
using UnityEngine;

namespace RobotGame.States
{
    public class EnemyStaggered : EnemyState
    {
        readonly private EnemyController enemy;
        readonly private float duration;
        private float currentTime = 0.0f;
        public EnemyStaggered(EnemyController enemy, float duration) { this.enemy = enemy; this.duration = duration; this.name = "EnemyStaggered"; }
        public override IEnumerator Start()
        {
            enemy.enemyAnimator.SetBool("isReacting", true);
            //enemy.GetComponent<SpriteRenderer>().color = Color.red;
            yield break;
        }

        public override IEnumerator Update()
        {
            if(currentTime < duration)
            {
                currentTime += Time.deltaTime;
                yield break;
            }
            
            int rand = Random.Range(0, 3);

            if (enemy.enemyType == EnemyType.Shy && rand == 0)
            {
                enemy.TransitionState(new EnemyFlee(enemy, enemy.fleeTime));
                yield break;
            }

            enemy.TransitionState(new EnemyFollow(enemy));
            yield break;
        }
        public override IEnumerator End()
        {
            enemy.enemyAnimator.SetBool("isReacting", false);
            //enemy.GetComponent<SpriteRenderer>().color = Color.white;
            yield break;
        }

    }
}