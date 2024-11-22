using RobotGame.States;
using UnityEngine;

public class Grapple : MonoBehaviour
{
    [SerializeField] private GameObject targetUI;
    [SerializeField] private float range;
    [SerializeField] private float speed;
    [SerializeField] private float grappleCooldownTime;
    [SerializeField] private float grappleAimMaxTime;

    private PlayerController playerController;
    private PlayerControls playerControls;
    private GameObject targetObject;

    private bool isAimingGrapple = false;
    private bool targetValid = false;

    private float grappleCooldownTimer;
    private float grappleAimTimer = 0.0f;
    private bool canGrapple = true;

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
            grappleAimTimer += Time.deltaTime;
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
        }
        if (isAimingGrapple && targetObject != null)
        {
            targetUI.SetActive(true);
            targetUI.transform.position = targetObject.transform.position;
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
            playerController.SetState(new PlayerGrappling(playerController, targetObject.transform.position, speed));
            targetObject.gameObject.transform.parent.GetComponentInChildren<EnemyHealth>().Stagger();
            grappleCooldownTimer = 0.0f;
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
        GameManager.Instance.SetSlowMoTimeScale();
        isAimingGrapple = true;
        Vector2 mousePosition = InputManager.Instance.GetMousePosition();
        Vector3 aimDirection = InputManager.Instance.GetAimDirection(playerController.transform.position);
        int layerMask = LayerMask.GetMask("Enemies") | LayerMask.GetMask("Grapple");

        Ray mouseRay = Camera.main.ScreenPointToRay(InputManager.Instance.GetMousePosition());
        RaycastHit raycast = new RaycastHit();
        Physics.Raycast(mouseRay, out raycast, 30, layerMask);
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
}
