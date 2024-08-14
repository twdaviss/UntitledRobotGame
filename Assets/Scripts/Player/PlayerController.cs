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
        if (moveDirection.magnitude > 0)
        {
            Debug.Log("hi");
        }
        transform.position += ((Vector3)moveDirection.normalized * moveSpeed * Time.deltaTime);

        playerAnimator.SetFloat("Horizontal", moveDirection.x);
        playerAnimator.SetFloat("Vertical", moveDirection.y);
        playerAnimator.SetFloat("Speed", moveDirection.SqrMagnitude());
    }

    public void PauseGame()
    {
        if (GameManager.Instance.isPauseMenuEnabled())
        {
            playerControls.Menu.Disable();
            playerControls.Gameplay.Enable();
            GameManager.Instance.DisablePauseMenu();
        }
        else
        {
            playerControls.Menu.Enable();
            playerControls.Gameplay.Disable();
            GameManager.Instance.EnablePauseMenu();
        }
    }

    public void UnPauseGame()
    {
        playerControls.Menu.Disable();
        playerControls.Gameplay.Enable();
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

        int layerMask = LayerMask.GetMask("Enemies") | LayerMask.GetMask("Grapple");

        Collider2D[] targets = Physics2D.OverlapCircleAll(mousePosition, 0.5f, layerMask);

        if (targets.Length == 0) 
        {
            slingArms.SetTarget(null);
        }
        if (targets.Length == 1)
        {
            slingArms.SetTarget(targets[0].gameObject);
            return;
        }

        float closestDistance = 100f;
        foreach (Collider2D target in targets)
        {
            float distanceFromMouse = Vector2.Distance(target.transform.position, mousePosition);

            if(distanceFromMouse < closestDistance)
            {
                closestDistance = distanceFromMouse;
                slingArms.SetTarget(target.gameObject);
            }
        }
    }

    public void StopLookingForSlingTarget()
    { 
        lookingForTarget = false;
    }

    private void OnEnable()
    {
        playerControls.Gameplay.Enable();

        GameManager.onUnPaused += UnPauseGame;
        playerControls.Gameplay.Pause.performed += ctx => PauseGame();
        playerControls.Menu.Pause.performed += ctx => PauseGame();

        playerControls.Gameplay.Move.performed += ctx => moveDirection = ctx.ReadValue<Vector2>();
        playerControls.Gameplay.Mouse.performed += ctx => mouseScreenPosition = ctx.ReadValue<Vector2>();
        playerControls.Gameplay.Move.canceled += ctx => moveDirection = Vector2.zero;
        playerControls.Gameplay.Fire.performed += ctx =>
        {
            scrapShot.ShootScrap();
        };
        playerControls.Gameplay.Magnetize.performed += ctx =>
        {
            scrapShot.MagnetizeScrap();
        };
        playerControls.Gameplay.Melee.performed += ctx => melee.Attack();
        playerControls.Gameplay.Sling.performed += ctx => slingArms.SlingStart();
        playerControls.Gameplay.Sling.performed += ctx => lookingForTarget = true;

        playerControls.Gameplay.Sling.canceled += ctx => slingArms.SlingReleased();
        playerControls.Gameplay.Sling.canceled += ctx => StopLookingForSlingTarget();
    }

    private void OnDisable()
    {
        GameManager.onUnPaused -= UnPauseGame;

        playerControls.Gameplay.Pause.performed -= ctx => PauseGame();
        playerControls.Menu.Pause.performed -= ctx => PauseGame();

        playerControls.Gameplay.Move.performed -= ctx => moveDirection = ctx.ReadValue<Vector2>();
        playerControls.Gameplay.Mouse.performed -= ctx => mouseScreenPosition = ctx.ReadValue<Vector2>();
        playerControls.Gameplay.Move.canceled -= ctx => moveDirection = Vector2.zero;
        playerControls.Gameplay.Fire.performed -= ctx =>
        {
            scrapShot.ShootScrap();
        };
        playerControls.Gameplay.Magnetize.performed -= ctx =>
        {
            scrapShot.MagnetizeScrap();
        };
        playerControls.Gameplay.Melee.performed -= ctx => melee.Attack();
        playerControls.Gameplay.Sling.performed -= ctx => slingArms.SlingStart();
        playerControls.Gameplay.Sling.performed -= ctx => lookingForTarget = true;

        playerControls.Gameplay.Sling.canceled -= ctx => slingArms.SlingReleased();
        playerControls.Gameplay.Sling.canceled -= ctx => StopLookingForSlingTarget();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        TransitionState(new PlayerDefault(this));
    }
}
