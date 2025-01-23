using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RobotGame.States
{
    public class EnemyFlee : EnemyState
    {
        readonly EnemyController enemy;
        public EnemyFlee(EnemyController enemy) { this.enemy = enemy; this.name = "EnemyFlee"; }

        private List<Vector3> path = null;
        private int numRays = 8;
        private int minDistance = 15;

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
            if(enemy.TargetMoved())
            {
                FindNewPoint();
            }
//
            //if (path == nul//
            ///
            //    FindNewPoint(//
            ///
            //else if (path.Count == //
            ///
            //    FindNewPoint(//
            //}
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
            if(distFromPlayer >= minDistance)
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
                RaycastHit2D hit = Physics2D.Raycast(enemy.transform.position, Quaternion.AngleAxis(ray * 360 / numRays, Vector3.forward) * Vector2.up, 10, layerMask);
                Debug.DrawRay(enemy.transform.position, Quaternion.AngleAxis(ray * 360 / numRays, Vector3.forward) * Vector2.up * 10);
                if (hit.collider != null)
                {
                    points[ray-1] = hit.point;
                    noObstacles = false;
                }
                else
                {
                    points[ray-1] = enemy.transform.position + (Quaternion.AngleAxis(ray * 360 / numRays, Vector3.forward) * Vector2.up).normalized * 10;
                }
            }

            if(noObstacles)
            {
                destination = (Vector2)enemy.transform.position + dirFromPlayer * 10;
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