using System.Collections;
using UnityEngine;

namespace RobotGame.States
{
    public class PlayerMelee : PlayerState
    {
        readonly PlayerController player;
        public PlayerMelee(PlayerController player, float radius, float damage, float knockBack, float duration, float staggerTime) { this.player = player; name = "PlayerMelee"; this.radius = radius; this.damage = damage; this.knockBack = knockBack; this.duration = duration; this.staggerTime = staggerTime; }

        private float radius;
        private float damage;
        private float knockBack;
        private float duration;
        private float currentTime = 0.0f;
        private float staggerTime;
        private bool staggerTriggered = false;

        int layerMask = LayerMask.GetMask("Enemies");

        public override IEnumerator Start()
        {
            player.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
            player.gameObject.GetComponent<SpriteRenderer>().color = Color.blue;
            yield break;
        }

        public override IEnumerator Update()
        {
            if(currentTime < duration) 
            {
                Collider2D[] targets = Physics2D.OverlapCircleAll(player.transform.position, radius, layerMask);
                foreach (Collider2D target in targets)
                {
                    if (target.gameObject.GetComponent<EnemyController>() != null)
                    {
                        target.gameObject.GetComponent<EnemyController>().KnockBack(knockBack, damage, (target.transform.position - player.transform.position).normalized);
                        //if(!staggerTriggered)
                        //{
                        //    GameManager.Instance.FreezeTimeScale(staggerTime);
                        //    staggerTriggered = true;
                        //}
                    }
                }
                currentTime += Time.deltaTime;
            }
            else
            {
                player.TransitionState(new PlayerDefault(player));
            }
            yield break;
        }

        public override IEnumerator FixedUpdate()
        {
            yield break;
        }

        public override IEnumerator End()
        {
            player.gameObject.GetComponent<SpriteRenderer>().color = Color.white;
            yield break;
        }
    }
}