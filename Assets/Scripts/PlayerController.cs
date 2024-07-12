using RobotGame.States;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : PlayerStateMachine
{
    [SerializeField] private float defaultMoveSpeed;
    [SerializeField] private PlayerControls playerControls;
    private Rigidbody2D playerRigidbody;
    private Vector2 moveDirection;
    private SpriteRenderer playerSprite;
    private Animator playerAnimator;

    private bool isSprinting = false;
    private float moveSpeed;

    private void Awake()
    {
        playerRigidbody = GetComponent<Rigidbody2D>();
        playerSprite = GetComponent<SpriteRenderer>();
        playerAnimator = GetComponent<Animator>();
        playerControls = new PlayerControls();

        SetState(new PlayerDefault(this));
    }

    void Update()
    {
        StartCoroutine(State.Update());
        playerAnimator.SetFloat("Horizontal", moveDirection.x);
        playerAnimator.SetFloat("Vertical", moveDirection.y);
        playerAnimator.SetFloat("Speed", moveDirection.SqrMagnitude());
    }

    private void FixedUpdate()
    {
        playerRigidbody.velocity = new Vector2(moveDirection.x * moveSpeed, moveDirection.y * moveSpeed);
        StartCoroutine(State.FixedUpdate());
    }

    public void InputHandler()
    {
        playerControls.Gameplay.Move.performed += ctx => moveDirection = ctx.ReadValue<Vector2>();
        playerControls.Gameplay.Move.canceled += ctx => moveDirection = Vector2.zero;

        if (moveDirection.x < 0) { playerSprite.flipX = true; }
        else { playerSprite.flipX = false; }

        playerControls.Gameplay.Sprint.performed += ctx => {
            isSprinting = !isSprinting;
        };
        if(moveDirection.magnitude == 0) { isSprinting = false; }
        if (isSprinting) { moveSpeed = 2 * defaultMoveSpeed; }
        else { moveSpeed = defaultMoveSpeed; }
    }

    private void OnEnable()
    {
        playerControls.Gameplay.Enable();
    }

    private void OnDisable()
    {
        playerControls.Gameplay.Disable();
    }
}
