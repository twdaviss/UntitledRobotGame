using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RobotGame.States
{
    public class EnemyWander : EnemyState
    {
        readonly EnemyController enemy;
        private Vector2 startingPosition;
        private Vector2 destination;
        private float waitTime;
        private int multiplier;
        public EnemyWander(EnemyController enemy, float waitTime, int multiplier) { this.enemy = enemy; this.waitTime = waitTime; this.multiplier = multiplier; this.name = "EnemyWander"; }

        private List<Vector3> path = null;
        private float waitTimer = 0f;

        public override IEnumerator Start()
        {
            startingPosition = enemy.transform.position;
            destination = enemy.transform.position;
            yield break;
        }

        public override IEnumerator Update()
        {
            MovementHandler();
            yield break;
        }

        public override IEnumerator FixedUpdate()
        {
            if (path == null)
            {
                FindNewPoint();
            }
            else if(path.Count == 0)
            {
                FindNewPoint();
            }
            if (path == null)
            {
                Debug.Log("No Available Path");
                yield break;
            }
            if (enemy.CheckMeleeRange())
            {
                enemy.SetState(new EnemyMelee(enemy));
            }
            yield break;
        }

        private void MovementHandler()
        {
            if (path == null)
            {
                return;
            }
            else if (path.Count == 0)
            {
                return;
            }
            enemy.MoveToNextPoint();
        }

        private void FindNewPoint()
        {
            if(waitTimer > 0)
            {
                waitTimer -= Time.deltaTime;
                return;
            }

            waitTimer = waitTime;
            do
            {
                float distance = Vector2.Distance(enemy.transform.position, startingPosition);
                if (distance > multiplier)
                {
                    Vector2 dir = (startingPosition - (Vector2)enemy.transform.position).normalized;
                    destination = enemy.transform.position;
                    destination += dir * distance;
                    break;
                }
                destination = enemy.transform.position;
                destination.x += Random.Range(-multiplier, multiplier);
                destination.y += Random.Range(-multiplier, multiplier);
            } while (!enemy.CheckDestination(destination));

            enemy.GetPathToDestination(destination);
            path = enemy.GetActivePath();
        }

        public override IEnumerator End()
        {
            yield break;
        }
    }
}