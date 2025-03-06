using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

namespace RobotGame.States
{
    public class PlayerDefault : PlayerState
    {
        readonly PlayerController player;
        public PlayerDefault(PlayerController player) { this.player = player; name = "PlayerDefault"; }
        
        public override IEnumerator Start()
        {
            InputManager.onMelee += Attack;
            yield break;
        }

        public override IEnumerator Update()
        {
            player.InputHandler();
            yield break;
        }
        private void Attack()
        {
            player.playerMelee.Attack();
        }
        public override IEnumerator End()
        {
            InputManager.onMelee -= Attack;
            yield break;
        }
        public override IEnumerator FixedUpdate()
        {
            yield break;
        }
    }
}