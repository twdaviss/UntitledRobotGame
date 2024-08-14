using RobotGame.States;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : PlayerStateMachine
{
    [SerializeField] private float defaultMoveSpeed;
    
    private Rigidbody2D playerRigidbody;
    private InputHandler inputHandler;
    private float moveSpeed;
    
    public SpriteRenderer playerSprite;
    public Animator playerAnimator;
   
    public Vector2 moveDirection;

    public ScrapShot scrapShot;
    public SlingArms slingArms;
    public Melee melee;

    public Camera activeCamera;
    public bool isSprinting = false;
    public bool lookingForTarget = false;

    private void Awake()
    {
        inputHandler = GetComponent<InputHandler>();
        playerRigidbody = GetComponent<Rigidbody2D>();
        playerSprite = GetComponent<SpriteRenderer>();
        playerAnimator = GetComponent<Animator>();
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
        
        if(moveDirection.magnitude == 0) { isSprinting = false; }
        if (isSprinting) { moveSpeed = 2 * defaultMoveSpeed; }
        else { moveSpeed = defaultMoveSpeed; }

        if (lookingForTarget)
        {
            LookForSlingTarget();
        }
        transform.position += ((Vector3)moveDirection.normalized * moveSpeed * Time.deltaTime);

        playerAnimator.SetFloat("Horizontal", moveDirection.x);
        playerAnimator.SetFloat("Vertical", moveDirection.y);
        playerAnimator.SetFloat("Speed", moveDirection.SqrMagnitude());
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

    public Vector2 GetMousePosition()
    {
        return inputHandler.GetMousePosition();
    }

    public Vector2 GetMouseDirection()
    {
        return inputHandler.GetMouseDirection();
    }
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        TransitionState(new PlayerDefault(this));
    }
}
