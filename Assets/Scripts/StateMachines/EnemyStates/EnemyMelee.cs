using System.Collections;
using UnityEngine;

namespace RobotGame.States
{
    public class EnemyMelee : EnemyState
    {
        readonly EnemyController enemy;
        public EnemyMelee(EnemyController enemy) { this.enemy = enemy; this.name = "EnemyMelee"; }
        private float meleeTimer = 0.0f;
        private bool isBuildingUp = true;
        private bool isMeleeing = false;
        private bool isRecovering = false;
        public override IEnumerator Start()
        {
            yield break;
        }

        public override IEnumerator Update()
        {
            if (isBuildingUp)
            {
                if (meleeTimer > enemy.meleeBuildUpTime)
                {
                    isMeleeing = true;
                    isBuildingUp = false;
                    meleeTimer = 0.0f;
                    yield break;
                }
                enemy.GetComponent<SpriteRenderer>().color = Color.yellow;
            }
            else if (isMeleeing)
            {
                if (meleeTimer > enemy.meleeDamageTime)
                {
                    isMeleeing = false;
                    isRecovering = true;
                    meleeTimer = 0.0f;
                    yield break;
                }
                Melee();
                enemy.GetComponent<SpriteRenderer>().color = Color.red;
            }
            else if (isRecovering)
            {
                if (meleeTimer > enemy.meleeRecoveryTime)
                {
                    enemy.meleeCooldownTimer += enemy.meleeCooldown;
                    enemy.GetComponent<SpriteRenderer>().color = Color.black;
                    enemy.SetState(new EnemyFollow(enemy));
                    yield break;
                }
                enemy.GetComponent<SpriteRenderer>().color = Color.magenta;
            }

            meleeTimer += Time.deltaTime;
            yield break;
        }

        public override IEnumerator FixedUpdate()
        {
            yield break;
        }
        private void Melee()
        {
            int layerMask = LayerMask.GetMask("PlayerHitBox");
            Collider2D player = Physics2D.OverlapCircle(enemy.transform.position, enemy.meleeRange, layerMask);

            if (player != null)
            {
                player.GetComponentInChildren<PlayerHealth>().DealDamage(enemy.meleeDamage);
            }
        }
        public override IEnumerator End()
        {
            yield break;
        }
    }
}