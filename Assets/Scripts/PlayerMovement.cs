using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    private Rigidbody2D playerRigidbody;
    private Vector2 moveDirection;
    private SpriteRenderer playerSprite;
    private Animator playerAnimator;

    public InputActionReference move;

    private void Awake()
    {
        playerRigidbody = GetComponent<Rigidbody2D>();
        playerSprite = GetComponent<SpriteRenderer>();
        playerAnimator = GetComponent<Animator>();
    }

    void Update()
    {
        moveDirection = move.action.ReadValue<Vector2>();
        if (moveDirection.x < 0 && moveDirection.y <= 0) { playerSprite.flipX = true; }
        else { playerSprite.flipX = false; }
        playerAnimator.SetFloat("Horizontal", moveDirection.x);
        playerAnimator.SetFloat("Vertical", moveDirection.y);
        playerAnimator.SetFloat("Speed", moveDirection.SqrMagnitude());
    }

    private void FixedUpdate()
    {
        playerRigidbody.velocity = new Vector2(moveDirection.x * moveSpeed, moveDirection.y * moveSpeed);  
    }
}
