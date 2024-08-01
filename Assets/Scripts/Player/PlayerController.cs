using RobotGame.States;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : PlayerStateMachine
{
    [SerializeField] private float defaultMoveSpeed;
    [SerializeField] private PlayerControls playerControls;
    [SerializeField] public Camera activeCamera;
    [SerializeField] private GameObject mouseUI;
    private Vector2 moveDirection;
    private SpriteRenderer playerSprite;
    private Animator playerAnimator;
    private ScrapShot scrapShot;
    private SlingArms slingArms;
    private Melee melee;

    private bool isSprinting = false;
    private float moveSpeed;
    private Vector3 mouseScreenPosition;
    private bool lookingForTarget = false;

    private Rigidbody2D playerRigidbody;

    private void Awake()
    {
        playerRigidbody = GetComponent<Rigidbody2D>();
        playerSprite = GetComponent<SpriteRenderer>();
        playerAnimator = GetComponent<Animator>();
        playerControls = new PlayerControls();
        scrapShot = GetComponentInChildren<ScrapShot>();
        slingArms = GetComponentInChildren<SlingArms>();
        melee = GetComponentInChildren<Melee>();
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

        if (lookingForTarget)
        {
            LookForSlingTarget();
        }

        playerRigidbody.velocity = moveDirection * moveSpeed;

        playerAnimator.SetFloat("Horizontal", moveDirection.x);
        playerAnimator.SetFloat("Vertical", moveDirection.y);
        playerAnimator.SetFloat("Speed", moveDirection.SqrMagnitude());
    }

    public Vector2 GetMouseDirection()
    {
        Vector3 mouseDirection = (Vector2)activeCamera.ScreenToWorldPoint(mouseScreenPosition) - (Vector2)transform.position;
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

    public void LookForSlingTarget()
    {
        Vector2 mousePosition = GetMousePosition();
        mouseUI.SetActive(true);
        mouseUI.transform.position = mousePosition;

        int layerMask = LayerMask.GetMask("Enemies");

        Collider2D[] targets = Physics2D.OverlapCircleAll(mousePosition, 0.5f, layerMask);
        if (targets.Length == 1)
        {
            mouseUI.GetComponent<SpriteRenderer>().color = Color.green;
            return;
        }
        else if (targets.Length > 1)
        {
            mouseUI.GetComponent<SpriteRenderer>().color = Color.green;
            return;
        }
        else
        {
            mouseUI.GetComponent<SpriteRenderer>().color = Color.red;
            return;
        }
    }

    public void StopLookingForSlingTarget()
    { 
        mouseUI.SetActive(false);
        lookingForTarget = false;
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
        playerControls.Gameplay.Melee.performed += ctx => melee.Attack();
        playerControls.Gameplay.Sling.performed += ctx => slingArms.SlingStart();
        playerControls.Gameplay.Sling.performed += ctx => lookingForTarget = true;

        playerControls.Gameplay.Sling.canceled += ctx => slingArms.SlingReleased();
        playerControls.Gameplay.Sling.canceled += ctx => StopLookingForSlingTarget();

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
        playerControls.Gameplay.Melee.performed -= ctx => melee.Attack();
        playerControls.Gameplay.Sling.performed -= ctx => slingArms.SlingStart();
        playerControls.Gameplay.Sling.performed -= ctx => lookingForTarget = true;

        playerControls.Gameplay.Sling.canceled -= ctx => slingArms.SlingReleased();
        playerControls.Gameplay.Sling.canceled -= ctx => StopLookingForSlingTarget();
    }
}
