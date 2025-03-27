using System.Collections;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

namespace RobotGame.States
{
    public class EnemyIdle : EnemyState
    {
        readonly EnemyController enemy;
        public EnemyIdle(EnemyController enemy) { this.enemy = enemy; this.name = "EnemyIdle"; }
        
        public override IEnumerator Start()
        {
            

            //switch (enemy.enemyType)
            //{
            //    case EnemyType.Aggressive:
            //        enemy.SetState(new EnemyFollow(enemy));
            //        break;
            //    case EnemyType.Shy:
            //        enemy.SetState(new EnemyFollow(enemy));
            //        break;
            //    case EnemyType.Ranged:
            //        enemy.SetState(new EnemyFollow(enemy));
            //        break;
            //    case EnemyType.Explosive:
            //        break;
            //}
            yield break;
        }

        public override IEnumerator Update()
        {
            if (Vector2.Distance(enemy.transform.position, enemy.target.position) < enemy.aggroDistance)
            {
                enemy.SetState(new EnemyFollow(enemy));
            }
            yield break;
        }

        public override IEnumerator FixedUpdate()
        {
            yield break;
        }

        public override IEnumerator End()
        {
            yield break;
        }
    }
}