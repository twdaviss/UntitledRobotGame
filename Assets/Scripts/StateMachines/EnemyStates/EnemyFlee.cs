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
            Vector2 destination = new Vector2();
            LayerMask layerMask = LayerMask.GetMask("Obstacles");
            //layerMask |= LayerMask.GetMask("Player");
            Vector2[] points = new Vector2[numRays];
            for(int ray = 1; ray <= numRays; ++ray)
            {
                RaycastHit2D hit = Physics2D.Raycast(enemy.transform.position, Quaternion.AngleAxis(ray * 360 / numRays, Vector3.forward) * Vector2.up, 10, layerMask);
                Debug.DrawRay(enemy.transform.position, Quaternion.AngleAxis(ray * 360 / numRays, Vector3.forward) * Vector2.up * 10);
                if (hit.point != Vector2.zero)
                {
                    points[ray-1] = hit.point;
                }
                else
                {
                    points[ray-1] = enemy.transform.position += (Quaternion.AngleAxis(ray * 360 / numRays, Vector3.forward) * Vector2.up) * 10;
                }
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