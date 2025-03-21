using RobotGame.States;
using UnityEngine;

public class Grapple : MonoBehaviour
{
    [SerializeField] private GameObject targetUI;
    [SerializeField] private float range;
    [SerializeField] private float startingSpeed;
    [SerializeField] private float targetSpeed;
    [SerializeField] private float grappleCooldownTime;
    [SerializeField] private float grappleAimMaxTime;
    [SerializeField] private float targetUIOffset;
    [SerializeField] private AudioClip grappleStart;
    [SerializeField] private AudioClip grappleEnd;

    private PlayerController playerController;
    private GameObject targetObject;

    private bool isAimingGrapple = false;
    private bool targetValid = false;

    private float grappleCooldownTimer;
    private float grappleAimTimer = 0.0f;
    private bool canGrapple = true;
    private bool canGrappleAudio = true;
    private void Awake()
    {
        grappleCooldownTimer = grappleCooldownTime;
        playerController = GetComponentInParent<PlayerController>();
    }

    private void Update()
    {
        grappleCooldownTimer += Time.deltaTime;
        if (InputManager.playerControls.Gameplay.Grapple.inProgress)
        {
            canGrapple = true;
        }
        else
        {
            grappleAimTimer = 0.0f;
            canGrapple = false;
        }

        if (grappleAimTimer >= grappleAimMaxTime)
        {
            CancelGrapple();
            return;
        }

        if (canGrapple)
        {
            LookForTarget();
        }
        else
        {
            GrappleTarget();
            canGrappleAudio = true;
        }
        if (isAimingGrapple && targetObject != null)
        {
            targetUI.SetActive(true);
            Vector3 position = targetObject.transform.position;
            position.z -= targetUIOffset;
            targetUI.transform.position = position;
        }
        else
        {
            targetUI.SetActive(false);
        }
    }

    private void FixedUpdate()
    {
        GameManager.Instance.SetGrappleCooldownUI(grappleCooldownTimer/ grappleCooldownTime);
    }

    public void GrappleTarget()
    {
        if (grappleCooldownTimer < grappleCooldownTime){return;}
        if(isAimingGrapple == false) { return; }
        
        isAimingGrapple = false;
        if (targetValid)
        {
            if (targetObject.GetComponentInParent<EnemyController>() != null)
            {
                targetObject.gameObject.transform.parent.GetComponentInChildren<EnemyHealth>().Stagger(0.6f);
            }

            grappleCooldownTimer = 0.0f;
            playerController.TransitionState(new PlayerGrappling(playerController, targetObject.transform.position, startingSpeed, targetSpeed));
        }
        
        StartCoroutine(GameManager.Instance.ResetTimeScale());
        isAimingGrapple = false;
        targetObject = null;
    }

    public void CancelGrapple()
    {
        StartCoroutine(GameManager.Instance.ResetTimeScale());
        isAimingGrapple = false;
        targetObject = null;
        targetUI.SetActive(false);
    }

    public void LookForTarget()
    {
        if (grappleCooldownTimer < grappleCooldownTime)
        {
            return;
        }
        if (canGrappleAudio)
        {
            GetComponent<AudioSource>().pitch = 2;
            GetComponent<AudioSource>().PlayOneShot(grappleStart);
            canGrappleAudio = false;
        }
        GameManager.Instance.SetSlowMoTimeScale();
        isAimingGrapple = true;
        Vector2 mousePosition = InputManager.Instance.GetMousePosition();
        Vector3 aimDirection = InputManager.Instance.GetAimDirection(playerController.transform.position);
        int layerMask = LayerMask.GetMask("Enemies") | LayerMask.GetMask("Grapple");

        Ray mouseRay = Camera.main.ScreenPointToRay(InputManager.Instance.GetMousePosition());
        RaycastHit raycast = new RaycastHit();
        Physics.Raycast(mouseRay, out raycast, 120, layerMask);
        //Debug.DrawLine(mouseRay.origin, mouseRay.origin + mouseRay.direction * 30, Color.red);

        if (raycast.point == null || raycast.point == Vector3.zero)
        {
            targetObject = null;
            targetValid = false;
            return;
        }

        if (Vector3.Distance(playerController.transform.position, raycast.point) < range)
        {
            targetObject = raycast.transform.gameObject;
            targetUI.GetComponent<SpriteRenderer>().color = Color.green;
            targetValid = true;
            return;
        }
        else
        {
            targetObject = raycast.transform.gameObject;
            targetValid = false;
            targetUI.GetComponent<SpriteRenderer>().color = Color.red;
        }
    }

    public void PlayGrappleEnd()
    {
        GetComponent<AudioSource>().pitch = 2;
        GetComponent<AudioSource>().PlayOneShot(grappleEnd);
    }
}
