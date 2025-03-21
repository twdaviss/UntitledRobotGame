using System.Collections;
using UnityEngine;

namespace RobotGame.States
{
    public class EnemyShoot : EnemyState
    {
        readonly EnemyController enemy;
        public EnemyShoot(EnemyController enemy) { this.enemy = enemy; this.name = "EnemyShoot"; }
        private float shootTimer = 0.0f;
        private bool isBuildingUp = true;
        private bool isShooting = false;
        private bool isRecovering = false;
        private bool canShoot = true;

        public override IEnumerator Start()
        {
            yield break;
        }

        public override IEnumerator Update()
        {
            if (isBuildingUp)
            {
                enemy.enemyAnimator.SetBool("isBuildingUp", true);
                if (shootTimer > enemy.shootBuildUpTime)
                {
                    isShooting = true;
                    isBuildingUp = false;
                    shootTimer = 0.0f;
                    enemy.enemyAnimator.SetBool("isBuildingUp", false);
                    yield break;
                }
            }
            else if (isShooting)
            {
                enemy.enemyAnimator.SetBool("isShooting", true);
                if (shootTimer > enemy.shootTime)
                {
                    isShooting = false;
                    isRecovering = true;
                    shootTimer = 0.0f;
                    enemy.enemyAnimator.SetBool("isShooting", false);
                    yield break;
                }
                if(canShoot)
                {
                    enemy.Shoot((enemy.target.position - enemy.transform.position).normalized, enemy.projectileSpeed);
                    canShoot = false;
                }
            }
            else if (isRecovering)
            {
                enemy.enemyAnimator.SetBool("isReacting", true);
                if (shootTimer > enemy.shootRecoveryTime)
                {
                    enemy.shootCooldownTimer += enemy.shootCooldown;
                    enemy.TransitionState(new EnemyIdle(enemy));
                    enemy.enemyAnimator.SetBool("isReacting", false);
                    yield break;
                }
            }

            shootTimer += Time.deltaTime;
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