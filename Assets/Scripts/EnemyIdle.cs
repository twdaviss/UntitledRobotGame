using System.Collections;
using UnityEngine;

namespace RobotGame.States
{
    public class EnemyIdle : EnemyState
    {
        readonly Enemy enemy;
        public EnemyIdle(Enemy enemy) { this.enemy = enemy; name = "EnemyIdle"; }
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
            if(enemy.GetActivePath() != null) {
                Vector3 moveTarget = enemy.GetActivePath()[enemy.CurrentPathIndex];
                if (Vector3.Distance(enemy.transform.position, moveTarget) > 0.5f)
                {
                    Vector3 moveDirection = (moveTarget - enemy.transform.position).normalized;

                    enemy.transform.position = enemy.transform.position + moveDirection * enemy.moveSpeed * Time.deltaTime;
                }
                else
                {
                    enemy.CurrentPathIndex++;
                    if(enemy.CurrentPathIndex >= enemy.GetActivePath().Count)
                    {
                        StopMoving();
                    }
                }
            }
        }
        private void StopMoving()
        {
            enemy.ClearPath();
        }
    }
}