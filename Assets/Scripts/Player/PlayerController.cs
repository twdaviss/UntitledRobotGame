using RobotGame.States;
using UnityEngine;

public class PlayerController : PlayerStateMachine
{
    [SerializeField] private float defaultMoveSpeed;
    [SerializeField] private AudioClip footsteps;
   
    private float moveSpeed;

    [HideInInspector] public SpriteRenderer playerSprite;
    [HideInInspector] public Animator playerAnimator;
    [HideInInspector] public AudioSource playerAudioSource;
    [HideInInspector] public Melee playerMelee;
    [HideInInspector] public Vector2 moveDirection;
    [HideInInspector] public Vector2 prevDirection;

    private void Awake()
    {
        playerSprite = GetComponent<SpriteRenderer>();
        playerAnimator = GetComponent<Animator>();
        playerAudioSource = GetComponent<AudioSource>();
        playerMelee = GetComponentInChildren<Melee>();

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

    public void ToggleAnimator(bool toggle)
    {
        playerAnimator.enabled = toggle;
    }

    public void InputHandler()
    {
        moveDirection = InputManager.Instance.GetMoveDirection();
        if(moveDirection.magnitude > 0.5)
        {
            prevDirection = moveDirection;
            if (!playerAudioSource.isPlaying)
            {
                playerAudioSource.Play();
            }
        }
        else
        {
            playerAudioSource.Stop();
        }
        
        moveSpeed = defaultMoveSpeed;
        if (!GetComponentInChildren<Grapple>().CheckGrappling())
        {
            transform.position += (Vector3)moveDirection.normalized * moveSpeed * Time.deltaTime;
        }
        playerAnimator.SetFloat("Horizontal", moveDirection.x);
        playerAnimator.SetFloat("Vertical", moveDirection.y);
        playerAnimator.SetFloat("Speed", moveDirection.SqrMagnitude());

        playerAnimator.SetFloat("PrevHorizontal", prevDirection.x);
        playerAnimator.SetFloat("PrevVertical", prevDirection.y);

        if (moveDirection.magnitude < 0.1)
        {
            return;
        }
        if (moveDirection.x < 0 || prevDirection.x < 0) { playerSprite.flipX = false; }
        else { playerSprite.flipX = true; }
    }

    public string GetCurrentState()
    {
        return State.name;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("EnemyProjectiles"))
        {
            GetComponentInChildren<PlayerHealth>().DealDamage(10);
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        StartCoroutine(State.OnCollisionEnter2D(collision));
    }
}
