using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements.Experimental;
using static InputManager;

namespace RobotGame.States
{
    public class PlayerGrappling : PlayerState
    {
        private readonly PlayerController player;
        private readonly Vector3 endPoint;
        private Rigidbody2D playerRigidbody;
        private Vector2 direction;
        private float startingSpeed;
        private float targetSpeed;
        private float totalDistance;

        public PlayerGrappling(PlayerController player, Vector3 grappleEndPoint, float startingSpeed, float targetSpeed) { this.player = player; name = "PlayerSlinging"; this.endPoint = grappleEndPoint; this.startingSpeed = startingSpeed; this.targetSpeed = targetSpeed; }
        
        public override IEnumerator Start()
        {
            player.playerAnimator.SetBool("isBuildingUp", false);
            player.playerAnimator.SetBool("isGrappling", true);
            player.GetComponentInChildren<Grapple>().PlayGrappleEnd();
            playerRigidbody = player.GetComponent<Rigidbody2D>();
            direction = (endPoint - player.transform.position).normalized;
            totalDistance = Vector3.Distance(player.transform.position, endPoint);
            InputManager.onMelee += Attack;
            yield break;
        }

        public override IEnumerator Update()
        {
            float distance = Vector3.Distance(player.transform.position, endPoint);
            float t = (totalDistance - distance)/ totalDistance;
            float speed = startingSpeed + (targetSpeed * Easing.OutCubic(t));
            if (distance < 0.75f) 
            {
                player.TransitionState(new PlayerDefault(player));
                Debug.Log(speed);
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
            player.playerAnimator.SetBool("isGrappling", false);
            InputManager.onMelee -= Attack;
            playerRigidbody.velocity = Vector2.zero;
            yield break;
        }

        private void Attack()
        {
            float distance = Vector3.Distance(player.transform.position, endPoint);
            if(distance < 2)
            {
                player.playerMelee.Attack();
            }
        }

        public override IEnumerator OnCollisionEnter2D(Collision2D collision)
        {
            if (this.GetType() == typeof(PlayerGrappling))
            {
                player.TransitionState(new PlayerDefault(player));
            }
            yield break;
        }
    }
}