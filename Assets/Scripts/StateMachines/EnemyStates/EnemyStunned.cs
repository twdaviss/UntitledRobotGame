using System.Collections;
using UnityEngine;

namespace RobotGame.States
{
    public class EnemyStunned : EnemyState
    {
        readonly EnemyController enemy;
        public EnemyStunned(EnemyController enemy, float stunTime) { this.enemy = enemy; this.stunTime = stunTime; this.name = "EnemyStunned"; }
        
        private float stunTime;
        private float stunTimer;
        public override IEnumerator Start()
        {
            enemy.isStunned = true;
            enemy.ClearPath();
            enemy.enemyAnimator.SetBool("isStunned", true);
            enemy.enemyAnimator.SetBool("isReacting", false);
            enemy.enemyAnimator.SetBool("isAttacking", false);
            enemy.enemyAnimator.SetBool("isBuildingUp", false);
            enemy.enemyAnimator.SetBool("isShooting", false);

            yield break;
        }

        public override IEnumerator Update()
        {
            stunTimer += Time.deltaTime;
            if(stunTimer > stunTime)
            {
                enemy.TransitionState(new EnemyIdle(enemy));
            }
            yield break;
        }

        public override IEnumerator FixedUpdate()
        {
            yield break;
        }

        public override IEnumerator End()
        {
            enemy.isStunned = false;
            yield break;
        }
    }
}