using System.Collections;
using UnityEngine;

namespace RobotGame.States
{
    public class EnemyFollow : EnemyState
    {
        readonly EnemyController enemy;
        public EnemyFollow(EnemyController enemy) { this.enemy = enemy; name = "EnemyIdle"; }
        private Vector3 moveTarget;
        public override IEnumerator Start()
        {
            yield break;
        }

        public override IEnumerator Update()
        {
            MovementHandler();
            yield break;
        }

        public override IEnumerator FixedUpdate()
        {
            if(enemy.GetActivePath().Count > 0)
            {
                enemy.CheckForTarget();
            }
            yield break;
        }

        private void MovementHandler()
        {
            moveTarget = enemy.GetActivePath()[0];
            if (Vector3.Distance(enemy.transform.position, moveTarget) < 1.4f)
            {
                enemy.GetActivePath().RemoveAt(0);

                if (enemy.GetActivePath().Count <= 0)
                {
                    StopMoving();
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