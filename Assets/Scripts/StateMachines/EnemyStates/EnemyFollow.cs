using System.Collections;
using UnityEngine;

namespace RobotGame.States
{
    public class EnemyFollow : EnemyState
    {
        private readonly EnemyController enemy;
        private Vector3 moveTarget;
        public EnemyFollow(EnemyController enemy) { this.enemy = enemy; name = "EnemyFollow"; }
        public override IEnumerator Start()
        {
            enemy.StopMoving();
            yield break;
        }

        public override IEnumerator Update()
        {
            MovementHandler();
            yield break;
        }

        public override IEnumerator FixedUpdate()
        {
            if(enemy.GetActivePath() == null)
            {
                Debug.Log("No Available Path");
                yield break;
            }
            if(enemy.GetActivePath().Count <= 0)
            {
                yield break;
            }
            if(enemy.CheckForTarget())
            {
                if (enemy.meleeCooldownTimer <= 0)
                {
                    enemy.SetState(new EnemyMelee(enemy));
                }
            }
            yield break;
        }

        private void MovementHandler()
        {
            if(enemy.GetActivePath() == null)
            {
                Debug.Log("No Available Path");
                return;
            }
            moveTarget = enemy.GetActivePath()[0];
            if (Vector3.Distance(enemy.transform.position, moveTarget) < 1.5f)
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
        public override IEnumerator End()
        {
            enemy.ClearPath();
            yield break;
        }

        private void StopMoving()
        {
            enemy.ClearPath();
        }
    }
}