using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

namespace RobotGame.States
{
    public class PlayerDefault : PlayerState
    {
        readonly PlayerController player;
        public PlayerDefault(PlayerController player) { this.player = player; name = "NewPlayerState"; }
        
        public override IEnumerator Start()
        {
            yield break;
        }

        public override IEnumerator Update()
        {
            player.InputHandler();
            yield break;
        }

        public override IEnumerator FixedUpdate()
        {
            yield break;
        }
    }
}