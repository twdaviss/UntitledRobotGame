using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RobotGame.States
{
    public class EnemyFollow : EnemyState
    {
        private readonly EnemyController enemy;
        public EnemyFollow(EnemyController enemy) { this.enemy = enemy; name = "EnemyFollow"; }

        private List<Vector3> path;
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
            enemy.CheckTarget();
            path = enemy.GetActivePath();
            if(path == null)
            {
                Debug.Log("No Available Path");
                yield break;
            }
            if(enemy.CheckMeleeRange())
            {
                enemy.SetState(new EnemyMelee(enemy));
            }
            yield break;
        }

        private void MovementHandler()
        {
            if(path == null || path.Count == 0)
            {
                return;
            }
            enemy.MoveToNextPoint();
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