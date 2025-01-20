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
        private int numRays = 6;

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
            Vector2 destination;
            LayerMask layerMask = LayerMask.GetMask("Obstacles");
            layerMask |= LayerMask.GetMask("Player");
            RaycastHit2D[] hits = new RaycastHit2D[8];
            
            //hits[0] = Physics2D.Raycast(enemy.transform.position, new Vector2( 0.0f,  1.0f), 10.0f, layerMask);
            //hits[1] = Physics2D.Raycast(enemy.transform.position, new Vector2( 0.5f,  0.5f), 10.0f, layerMask);
            //hits[2] = Physics2D.Raycast(enemy.transform.position, new Vector2( 1.0f,  0.0f), 10.0f, layerMask);
            //hits[3] = Physics2D.Raycast(enemy.transform.position, new Vector2( 0.5f, -0.5f), 10.0f, layerMask);
            //hits[4] = Physics2D.Raycast(enemy.transform.position, new Vector2( 0.0f, -1.0f), 10.0f, layerMask);
            //hits[5] = Physics2D.Raycast(enemy.transform.position, new Vector2(-0.5f, -0.5f), 10.0f, layerMask);
            //hits[6] = Physics2D.Raycast(enemy.transform.position, new Vector2(-1.0f,  0.0f), 10.0f, layerMask);
            //hits[7] = Physics2D.Raycast(enemy.transform.position, new Vector2(-0.5f,  0.5f), 10.0f, layerMask);

            //has to be a better way to do this

            if (hits.Count() == 0)
            {
                return;
            }
            destination = hits[0].point;
            foreach (RaycastHit2D hit in hits)
            {
                if(hit.point == null) continue;
                if(Vector2.Distance(enemy.target.position, hit.point) > Vector2.Distance(enemy.target.position, destination))
                {
                    destination = hit.point;
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