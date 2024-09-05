using System.Collections;
using UnityEngine;

namespace RobotGame.States
{
    public class PlayerGrappling : PlayerState
    {
        private readonly PlayerController player;
        private readonly Vector3 endPoint;
        private Rigidbody2D playerRigidbody;
        private Vector2 direction;
        private float speed;

        public PlayerGrappling(PlayerController player, Vector3 grappleEndPoint, float speed) { this.player = player; name = "PlayerSlinging"; this.endPoint = grappleEndPoint; this.speed = speed; }
        
        public override IEnumerator Start()
        {
            playerRigidbody = player.GetComponent<Rigidbody2D>();
            direction = (endPoint - player.transform.position).normalized;
            yield break;
        }

        public override IEnumerator Update()
        {
            if(Vector3.Distance(player.transform.position, endPoint) < 0.5f) 
            {
                player.TransitionState(new PlayerDefault(player));
                player.grappleCooldownTimer = 0.0f;
            }
            playerRigidbody.transform.position += (Vector3)direction * speed * Time.deltaTime;
            yield break;
        }

        public override IEnumerator FixedUpdate()
        {
            yield break;
        }

        public override IEnumerator End()
        {
            playerRigidbody.velocity = Vector2.zero;
            yield break;
        }
    }
}