using System.Collections;
using UnityEngine;

namespace RobotGame.States
{
    public class PlayerMelee : PlayerState
    {
        readonly PlayerController player;
        public PlayerMelee(PlayerController player, float radius, float damage, float stun, float knockBack, float duration) { this.player = player; name = "PlayerMelee"; this.radius = radius; this.damage = damage; this.stun = stun; this.knockBack = knockBack; this.duration = duration;}

        private float radius;
        private float damage;
        private float stun;
        private float knockBack;
        private float duration;
        private float currentTime = 0.0f;

        int layerMask = LayerMask.GetMask("Enemies");

        public override IEnumerator Start()
        {
            player.GetComponent<Rigidbody2D>().velocity = Vector3.zero;

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
                        target.gameObject.GetComponent<EnemyController>().Damage(damage, stun, knockBack, (target.transform.position - player.transform.position).normalized);
                    }
                }
                currentTime += Time.deltaTime;
            }
            else
            {
                player.playerAnimator.SetBool("isMeleeing", false);
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
            player.playerAnimator.SetBool("isMeleeing", false);
            player.gameObject.GetComponent<SpriteRenderer>().color = Color.white;
            yield break;
        }
    }
}