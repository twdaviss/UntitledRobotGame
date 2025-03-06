using System.Collections;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

namespace RobotGame.States
{
    public class EnemyKnockback : EnemyState
    {
        readonly EnemyController enemy;
        private float knockbackSpeed;
        private Vector2 direction;
        private float knockbackTime = 0.2f;
        private float knockbackTimer = 0.0f;
        public EnemyKnockback(EnemyController enemy, float knockbackSpeed, Vector2 direction) { this.enemy = enemy; this.knockbackSpeed = knockbackSpeed; this.direction = direction; this.name = "EnemyKnockback"; }
        
        public override IEnumerator Start()
        {
            enemy.GetComponent<SpriteRenderer>().color = Color.red;
            yield break;
        }

        public override IEnumerator Update()
        {
            float speed = knockbackSpeed - ((knockbackTimer / knockbackTime) * knockbackSpeed);
            enemy.transform.position += (Vector3)direction * speed * Time.deltaTime;

            if(knockbackTimer < knockbackTime)
            {
                knockbackTimer += Time.deltaTime;
            }
            else
            {
                enemy.TransitionState(new EnemyStaggered(enemy));
            }
            yield break;
        }

        public override IEnumerator FixedUpdate()
        {
            yield break;
        }

        public override IEnumerator End()
        {
            enemy.GetComponent<SpriteRenderer>().color = Color.white;
            yield break;
        }
    }
}