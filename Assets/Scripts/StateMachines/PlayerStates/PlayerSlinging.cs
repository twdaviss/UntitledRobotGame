using System.Collections;
using UnityEngine;

namespace RobotGame.States
{
    public class PlayerSlinging : PlayerState
    {
        private readonly PlayerController player;
        private readonly Vector3 endPoint;
        private Vector2 direction;
        private float speed;

        public PlayerSlinging(PlayerController player, Vector3 slingEndPoint, float speed) { this.player = player; name = "PlayerSlinging"; this.endPoint = slingEndPoint; this.speed = speed; }
        
        public override IEnumerator Start()
        {
            direction = (endPoint - player.transform.position).normalized;
            yield break;
        }

        public override IEnumerator Update()
        {
            if(Vector3.Distance(player.transform.position, endPoint) < 0.5f) 
            {
                player.SetState(new PlayerDefault(player));
            }
            player.playerRigidbody.velocity = direction * speed;
            yield break;
        }

        public override IEnumerator FixedUpdate()
        {
            yield break;
        }
    }
}