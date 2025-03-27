using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RobotGame.States
{
    public class EnemyFlee : EnemyState
    {
        readonly EnemyController enemy;
        public EnemyFlee(EnemyController enemy, float fleeTime) { this.enemy = enemy; this.name = "EnemyFlee"; this.fleeTime = fleeTime; }

        private List<Vector3> path = null;
        private int numRays = 8;
        private float minDistance;
        private float fleeTime;
        private float fleeTimer = 0.0f;
        public override IEnumerator Start()
        {
            minDistance = enemy.fleeDistanceThreshold + 5;
            //FindNewPoint();
            yield break;
        }

        public override IEnumerator Update()
        {
            if (enemy.enemyType == EnemyType.Ranged)
            {
                if (enemy.CheckShootRange() && enemy.shootCooldownTimer <= 0)
                {
                    enemy.TransitionState(new EnemyShoot(enemy));
                }
            }
            float distance = Vector2.Distance(enemy.transform.position, enemy.target.position);
            if (distance > minDistance || fleeTimer > fleeTime)
            {
                switch (enemy.enemyType)
                {
                    case EnemyType.Aggressive:
                        //enemy.SetState(new EnemyFollow(enemy));
                        break;
                    case EnemyType.Shy:
                        enemy.SetState(new EnemyWander(enemy, enemy.wanderWaitTime, enemy.wanderMultiplier, enemy.wanderTime));
                        Debug.Log("Far enought away or timer finished. Switching to wander (distance/timer):" + distance);
                        break;
                    case EnemyType.Ranged:
                        enemy.SetState(new EnemyFollow(enemy));
                        break;
                    case EnemyType.Explosive:
                        break;
                }
            }
            fleeTimer += Time.deltaTime;
            MovementHandler();
            yield break;
        }

        public override IEnumerator FixedUpdate()
        {
            if(enemy.TargetMoved())
            {
                FindNewPoint();
            }
            
            if (path == null)
            {
                FindNewPoint();
            }
            else if (path.Count == 0)
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
            float distFromPlayer = Vector2.Distance(enemy.transform.position, enemy.target.position);
            if(distFromPlayer > minDistance)
            {
                return;
            }

            Vector2 dirFromPlayer = (enemy.transform.position - enemy.target.position).normalized;
            Vector2 destination;
            LayerMask layerMask = LayerMask.GetMask("Obstacles");

            //layerMask |= LayerMask.GetMask("Player");
            Vector2[] points = new Vector2[numRays];
            bool noObstacles = true;
            for(int ray = 1; ray <= numRays; ++ray)
            {
                RaycastHit2D hit = Physics2D.Raycast(enemy.transform.position, Quaternion.AngleAxis(ray * 360 / numRays, Vector3.forward) * Vector2.up, 5, layerMask);
                Debug.DrawRay(enemy.transform.position, Quaternion.AngleAxis(ray * 360 / numRays, Vector3.forward) * Vector2.up * 5);
                if (hit.collider != null)
                {
                    points[ray-1] = hit.point;
                    noObstacles = false;
                }
                else
                {
                    points[ray-1] = enemy.transform.position + (Quaternion.AngleAxis(ray * 360 / numRays, Vector3.forward) * Vector2.up).normalized * 5;
                }
            }

            if(noObstacles)
            {
                destination = (Vector2)enemy.transform.position + (dirFromPlayer * 10);
                enemy.GetPathToDestination(destination);
                path = enemy.GetActivePath();
                return;
            }

            if (points.Count() == 0)
            {
                return;
            }
            destination = points[0];
            foreach (Vector2 point in points)
            {
                if(point == null) continue;
                if(Vector2.Distance(enemy.target.position, point) > Vector2.Distance(enemy.target.position, destination))
                {
                    destination = point;
                }
            }
            if(destination == null) return; 

            enemy.GetPathToDestination(destination);
            path = enemy.GetActivePath();
        }

        public override IEnumerator End()
        {
            yield break;
        }
    }
}