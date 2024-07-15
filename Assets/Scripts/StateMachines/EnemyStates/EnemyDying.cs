using System.Collections;
using UnityEngine;

namespace RobotGame.States
{
    public class EnemyDying : EnemyState
    {
        readonly EnemyController enemy;
        public EnemyDying(EnemyController enemy) { this.enemy = enemy; name = "EnemyDying"; }
        
        public override IEnumerator Start()
        {
            enemy.StartCoroutine(enemy.Despawn());
            yield break;
        }

        public override IEnumerator Update()
        {
            yield break;
        }

        public override IEnumerator FixedUpdate()
        {
            yield break;
        }
    }
}