using System.Collections;
using UnityEngine;

namespace RobotGame.States
{
    public class PlayerStaggered : PlayerState
    {
        readonly PlayerController player;
        private float staggerTime;
        private float staggerTimer = 0.0f;
        public PlayerStaggered(PlayerController player, float staggerTime) { this.player = player; this.staggerTime = staggerTime; this.name = "PlayerStaggered"; }
        
        public override IEnumerator Start()
        {
            player.ToggleAnimator(false);
            player.gameObject.GetComponent<SpriteRenderer>().color = Color.grey;
            yield break;
        }

        public override IEnumerator Update()
        {
            if(staggerTimer < staggerTime)
            {
                staggerTimer += Time.deltaTime;
                yield break;
            }

            player.TransitionState(new PlayerDefault(player));
            yield break;
        }

        public override IEnumerator FixedUpdate()
        {
            yield break;
        }

        public override IEnumerator End()
        {
            player.ToggleAnimator(true);
            player.gameObject.GetComponent<SpriteRenderer>().color = Color.white;
            yield break;
        }
    }
}