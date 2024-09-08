using RobotGame.States;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : PlayerStateMachine
{
    public SpriteRenderer playerSprite;
    public Animator playerAnimator;
   
    public Vector2 moveDirection;

    [SerializeField] private float defaultMoveSpeed;
    
    private Rigidbody2D playerRigidbody;
    private InputManager inputHandler;
    private float moveSpeed;

    private void Awake()
    {
        inputHandler = GetComponent<InputManager>();
        playerRigidbody = GetComponent<Rigidbody2D>();
        playerSprite = GetComponent<SpriteRenderer>();
        playerAnimator = GetComponent<Animator>();

        SetState(new PlayerDefault(this));
    }
    void Update()
    {
        StartCoroutine(State.Update());
    }

    private void FixedUpdate()
    {
        StartCoroutine(State.FixedUpdate());
    }

    public void InputHandler()
    {
        moveDirection = InputManager.Instance.GetMoveDirection();
        if (moveDirection.x < 0) { playerSprite.flipX = true; }
        else { playerSprite.flipX = false; }
        
        if(InputManager.playerControls.Gameplay.Sprint.inProgress) { moveSpeed = 2 * defaultMoveSpeed; }
        else { moveSpeed = defaultMoveSpeed; }

        transform.position += (Vector3)moveDirection.normalized * moveSpeed * Time.deltaTime;

        playerAnimator.SetFloat("Horizontal", moveDirection.x);
        playerAnimator.SetFloat("Vertical", moveDirection.y);
        playerAnimator.SetFloat("Speed", moveDirection.SqrMagnitude());
    }

    public Vector2 GetMousePosition()
    {
        return InputManager.Instance.GetMousePosition();
    }

    public Vector2 GetMouseDirection()
    {
        return InputManager.Instance.GetMouseDirection(transform.position);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        StartCoroutine(State.OnCollisionEnter2D(collision));
    }
}
