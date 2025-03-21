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
        private float meleeCooldownTimer = 0.0f;

        public override IEnumerator Start()
        {
            yield break;
        }

        public override IEnumerator Update()
        {
            if (meleeCooldownTimer > 0.0f) { meleeCooldownTimer -= Time.deltaTime; }

            if (isBuildingUp)
            {
                enemy.enemyAnimator.SetBool("isBuildingUp", true);
                if (meleeTimer > enemy.meleeBuildUpTime)
                {
                    isMeleeing = true;
                    isBuildingUp = false;
                    meleeTimer = 0.0f;
                    enemy.enemyAnimator.SetBool("isBuildingUp", false);
                    enemy.PlayZapSound();
                    yield break;
                }
            }
            else if (isMeleeing)
            {
                enemy.enemyAnimator.SetBool("isAttacking", true);
                if (meleeTimer > enemy.meleeDamageTime)
                {
                    isMeleeing = false;
                    isRecovering = true;
                    meleeTimer = 0.0f;
                    enemy.enemyAnimator.SetBool("isAttacking", false);
                    yield break;
                }
                Melee();
            }
            else if (isRecovering)
            {
                enemy.enemyAnimator.SetBool("isReacting", true);
                if (meleeTimer > enemy.meleeRecoveryTime)
                {
                    meleeCooldownTimer += enemy.meleeCooldown;
                    enemy.TransitionState(new EnemyIdle(enemy));
                    enemy.enemyAnimator.SetBool("isReacting", false);
                    yield break;
                }
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