using RobotGame.States;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : PlayerStateMachine
{
    public SpriteRenderer playerSprite;
    public Animator playerAnimator;
    public AudioSource playerAudioSource;
    public Melee playerMelee;
    public Vector2 moveDirection;
    public Vector2 prevDirection;
    public float moveSpeed;

    [SerializeField] private float defaultMoveSpeed;
    [SerializeField] private AudioClip footsteps;
    

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
        if (moveDirection.x < 0 || prevDirection.x < 0) { playerSprite.flipX = false; }
        else { playerSprite.flipX = true; }
        
        if(InputManager.playerControls.Gameplay.Sprint.inProgress) { moveSpeed = 2 * defaultMoveSpeed; }
        else { moveSpeed = defaultMoveSpeed; }

        transform.position += (Vector3)moveDirection.normalized * moveSpeed * Time.deltaTime;

        playerAnimator.SetFloat("Horizontal", moveDirection.x);
        playerAnimator.SetFloat("Vertical", moveDirection.y);
        playerAnimator.SetFloat("Speed", moveDirection.SqrMagnitude());

        playerAnimator.SetFloat("PrevHorizontal", prevDirection.x);
        playerAnimator.SetFloat("PrevVertical", prevDirection.y);
    }

    public string GetCurrentState()
    {
        return State.name;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        StartCoroutine(State.OnCollisionEnter2D(collision));
    }
}
