using System.Collections;
using System.Collections.Generic;
using System.Net;
using Unity.VisualScripting;
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
        private float wanderTime;
        private float wanderTimer;

        public EnemyWander(EnemyController enemy, float waitTime, int multiplier, float wanderTime) { this.enemy = enemy; this.waitTime = waitTime; this.multiplier = multiplier; this.name = "EnemyWander"; this.wanderTime = wanderTime;}

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
            float distance = Vector2.Distance(enemy.transform.position, enemy.target.position);
            switch (enemy.enemyType)
            {
                case EnemyType.Aggressive:
                    break;
                case EnemyType.Shy:
                    if(distance < enemy.fleeDistanceThreshold)
                    {
                        enemy.SetState(new EnemyFlee(enemy, enemy.fleeTime));
                        Debug.Log("Too close. Switching to Flee (distance/timer):" + distance + "/" + wanderTimer);
                    }
                    else if (wanderTimer >= wanderTime)
                    {
                        enemy.SetState(new EnemyFollow(enemy));
                        Debug.Log("Done wandering. Switching to Follow (distance/timer):" + distance + "/" + wanderTimer);
                    }
                    break;
                case EnemyType.Ranged:
                    break;
                case EnemyType.Explosive:
                    break;
            }
            wanderTimer += Time.deltaTime;
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