using System.Collections;
using UnityEngine;

namespace RobotGame.States
{
    public class EnemyIdle : EnemyState
    {
        readonly Enemy enemy;
        public EnemyIdle(Enemy enemy) { this.enemy = enemy; name = "EnemyIdle"; }
        private Vector3 moveTarget;
        private float checkTargetTimer = 0f;
        private float checkTargetCooldown = 0.5f;
        public override IEnumerator Start()
        {
            yield break;
        }

        public override IEnumerator Update()
        {
            MovementHandler();
            yield break;
        }

        private void MovementHandler()
        {
            if(checkTargetTimer >= checkTargetCooldown)
            {
                checkTargetTimer = 0;
                enemy.CheckForTarget();
            }
            else
            {
                checkTargetTimer += Time.deltaTime;
            }

            Vector3 moveTarget = enemy.GetActivePath()[0];
            if (Vector3.Distance(enemy.transform.position, moveTarget) < 1.4f)
            {
                enemy.GetActivePath().RemoveAt(0);

                if (enemy.GetActivePath().Count <= 0)
                {
                    StopMoving();
                    enemy.CheckForTarget();
                    Debug.Log("Reached target");
                }
            }
            else
            {
                Vector3 moveDirection = (moveTarget - enemy.transform.position).normalized;
                enemy.transform.position = enemy.transform.position + (moveDirection * (enemy.moveSpeed * Time.deltaTime));
            }
        }

        private void StopMoving()
        {
            enemy.ClearPath();
        }
    }
}