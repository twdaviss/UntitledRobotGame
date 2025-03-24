using System.Collections;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

namespace RobotGame.States
{
    public class EnemyGrappled : EnemyState
    {
        readonly EnemyController enemy;
        public EnemyGrappled(EnemyController enemy, GameObject target, float startingSpeed, float targetSpeed) { this.enemy = enemy; this.target = target; this.startingSpeed = startingSpeed; this.targetSpeed = targetSpeed; this.name = "EnemyGrappled"; }
        Vector2 direction;
        float totalDistance;
        GameObject target;
        float startingSpeed;
        float targetSpeed;  

        public override IEnumerator Start()
        {
            yield break;
        }

        public override IEnumerator Update()
        {
            direction = (target.transform.position - enemy.transform.position).normalized;
            totalDistance = Vector3.Distance(enemy.transform.position, target.transform.position);

            float distance = Vector3.Distance(enemy.transform.position, target.transform.position);
            float t = (totalDistance - distance) / totalDistance;
            float speed = startingSpeed + (targetSpeed * Easing.OutCubic(t));
            if (distance < 2.0f)
            {
                enemy.TransitionState(new EnemyStunned(enemy, enemy.GetComponentInChildren<EnemyStun>().stunTime));
            }
            enemy.transform.position += (Vector3)direction * speed * Time.deltaTime;
            yield break;
        }

        public override IEnumerator FixedUpdate()
        {
            yield break;
        }

        public override IEnumerator End()
        {
            yield break;
        }
    }
}