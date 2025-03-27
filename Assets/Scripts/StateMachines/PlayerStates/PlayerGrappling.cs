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
        private readonly GameObject target;
        private Rigidbody2D playerRigidbody;
        private Vector2 direction;
        private float startingSpeed;
        private float targetSpeed;
        private float totalDistance;
        private bool meleePressed = false;

        public PlayerGrappling(PlayerController player, GameObject target, float startingSpeed, float targetSpeed) { this.player = player; name = "PlayerGrappling"; this.target = target; this.startingSpeed = startingSpeed; this.targetSpeed = targetSpeed; }
        
        public override IEnumerator Start()
        {
            player.playerAnimator.SetBool("isGrappling", true);
            player.GetComponentInChildren<Grapple>().PlayGrappleEnd();
            playerRigidbody = player.GetComponent<Rigidbody2D>();
            
            InputManager.onMelee += Attack;
            yield break;
        }

        public override IEnumerator Update()
        {
            direction = (target.transform.position - player.transform.position).normalized;
            totalDistance = Vector3.Distance(player.transform.position, target.transform.position);

            float distance = Vector3.Distance(player.transform.position, target.transform.position);
            float t = (totalDistance - distance)/ totalDistance;
            float speed = startingSpeed + (targetSpeed * Easing.OutCubic(t));
            if (distance < 2.0f) 
            {
                if (meleePressed)
                {
                    player.playerMelee.Attack();
                    Debug.Log("Combo achieved");
                }
                else
                {
                    player.TransitionState(new PlayerDefault(player));
                }
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
            InputManager.onMelee -= Attack;
            player.playerAnimator.SetBool("isGrappling", false);
            playerRigidbody.velocity = Vector2.zero;
            
            yield break;
        }

        private void Attack()
        {
            meleePressed = true;
            Debug.Log("Melee Pressed");
        }

        public override IEnumerator OnCollisionEnter2D(Collision2D collision)
        {
            if (meleePressed)
            {
                player.playerMelee.Attack();
                Debug.Log("Combo achieved");
            }
            else
            {
                player.TransitionState(new PlayerDefault(player));
            }
            yield break;
        }
    }
}