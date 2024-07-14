using RobotGame.States;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : PlayerStateMachine
{
    [SerializeField] private float defaultMoveSpeed;
    [SerializeField] private PlayerControls playerControls;
    [SerializeField] public Camera activeCamera;
    private Vector2 moveDirection;
    private SpriteRenderer playerSprite;
    private Animator playerAnimator;
    private ScrapShot scrapShot;
    private SlingArms slingArms;

    private bool isSprinting = false;
    private float moveSpeed;
    private Vector3 mouseScreenPosition;
    public Rigidbody2D playerRigidbody;

    private void Awake()
    {
        playerRigidbody = GetComponent<Rigidbody2D>();
        playerSprite = GetComponent<SpriteRenderer>();
        playerAnimator = GetComponent<Animator>();
        playerControls = new PlayerControls();
        scrapShot = GetComponentInChildren<ScrapShot>();
        slingArms = GetComponentInChildren<SlingArms>();

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
        if (moveDirection.x < 0) { playerSprite.flipX = true; }
        else { playerSprite.flipX = false; }

        playerControls.Gameplay.Sprint.performed += ctx => {
            isSprinting = !isSprinting;
        };
        if(moveDirection.magnitude == 0) { isSprinting = false; }
        if (isSprinting) { moveSpeed = 2 * defaultMoveSpeed; }
        else { moveSpeed = defaultMoveSpeed; }

        playerRigidbody.velocity = moveDirection * moveSpeed;

        playerAnimator.SetFloat("Horizontal", moveDirection.x);
        playerAnimator.SetFloat("Vertical", moveDirection.y);
        playerAnimator.SetFloat("Speed", moveDirection.SqrMagnitude());
    }

    public Vector2 GetMouseDirection()
    {
        Vector3 mouseDirection = (Vector2)activeCamera.ScreenToWorldPoint(mouseScreenPosition) - (Vector2)transform.position; // not working
        return mouseDirection.normalized;
    }

    public Vector3 GetMousePosition()
    {
        Vector3 mouseWorldPosition = (Vector2)activeCamera.ScreenToWorldPoint(mouseScreenPosition);
        return mouseWorldPosition;
    }

    public float GetMouseDistance()
    {
        return Vector3.Distance(transform.position, GetMousePosition());
    }

    private void OnEnable()
    {
        playerControls.Gameplay.Enable();

        playerControls.Gameplay.Move.performed += ctx => moveDirection = ctx.ReadValue<Vector2>();
        playerControls.Gameplay.Mouse.performed += ctx => mouseScreenPosition = ctx.ReadValue<Vector2>();
        playerControls.Gameplay.Move.canceled += ctx => moveDirection = Vector2.zero;
        playerControls.Gameplay.Fire.performed += ctx =>
        {
            scrapShot.ShootScrap();
        };
        playerControls.Gameplay.Sling.performed += ctx => slingArms.SlingStart();
        playerControls.Gameplay.Sling.canceled += ctx => slingArms.SlingReleased();

    }

    private void OnDisable()
    {
        playerControls.Gameplay.Disable();

        playerControls.Gameplay.Move.performed -= ctx => moveDirection = ctx.ReadValue<Vector2>();
        playerControls.Gameplay.Mouse.performed -= ctx => mouseScreenPosition = ctx.ReadValue<Vector2>();
        playerControls.Gameplay.Move.canceled -= ctx => moveDirection = Vector2.zero;
        playerControls.Gameplay.Fire.performed -= ctx =>
        {
            scrapShot.ShootScrap();
        };
        //playerControls.Gameplay.Sling.performed -= ctx => slingArms.Sling();
    }
}
