using System.Collections;
using UnityEngine;

namespace RobotGame.States
{
    public class EnemyKnockback : EnemyState
    {
        readonly EnemyController enemy;
        private float knockback;
        private Vector2 direction;
        private float knockbackTime = 1.0f;
        private float knockbackTimer = 0.0f;
        public EnemyKnockback(EnemyController enemy, float knockback, Vector2 direction) { this.enemy = enemy; this.knockback = knockback; this.direction = direction; this.name = "EnemyKnockback"; }
        
        public override IEnumerator Start()
        {
            enemy.GetComponent<SpriteRenderer>().color = Color.red;
            yield break;
        }

        public override IEnumerator Update()
        {
            enemy.transform.position += (Vector3)direction * knockback * Time.deltaTime;

            if(knockbackTimer < knockbackTime)
            {
                knockbackTimer += Time.deltaTime;
            }
            else
            {
                enemy.TransitionState(new EnemyFollow(enemy));
            }
            yield break;
        }

        public override IEnumerator FixedUpdate()
        {
            yield break;
        }

        public override IEnumerator End()
        {
            enemy.GetComponent<SpriteRenderer>().color = Color.black;
            yield break;
        }
    }
}