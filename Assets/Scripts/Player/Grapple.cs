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
        playerControls = InputManager.Instance.GetPlayerControls();
    }

    private void Update()
    {
        grappleCooldownTimer += Time.deltaTime;
        if (playerControls.Gameplay.Sling.inProgress)
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
            grappleCooldownTimer = 0.0f;
        }
        
        StartCoroutine(GameManager.Instance.ResetTimeScale());
        isAimingGrapple = false;
    }

    public void CancelGrapple()
    {
        StartCoroutine(GameManager.Instance.ResetTimeScale());
        isAimingGrapple = false;
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

        int layerMask = LayerMask.GetMask("Enemies") | LayerMask.GetMask("Grapple");

        Collider2D[] targets = Physics2D.OverlapCircleAll(mousePosition, 0.5f, layerMask);

        if (targets.Length == 0)
        {
            targetObject = null;
            return;
        }
        else if (targets.Length == 1)
        {
            targetObject = targets[0].gameObject;
        }
        else
        {
            float closestDistance = 100f;
            foreach (Collider2D target in targets)
            {
                float distanceFromMouse = Vector2.Distance(target.transform.position, mousePosition);

                if (distanceFromMouse < closestDistance)
                {
                    closestDistance = distanceFromMouse;
                    targetObject = target.gameObject;
                }
            }
        }
        float targetDistance = Vector3.Distance(playerController.transform.position, targetObject.transform.position);
        if (targetDistance > range || targetDistance < 4.0f)
        {
            targetUI.GetComponent<SpriteRenderer>().color = Color.red;
            targetValid = false;
            return;
        }

        layerMask = LayerMask.GetMask("Obstacles");
        Vector2 targetDirection = (targetObject.transform.position - transform.position).normalized;

        RaycastHit2D raycast = Physics2D.Raycast(playerController.transform.position, targetDirection, range, layerMask);
        if (raycast.point != null && raycast.point != Vector2.zero)
        {
            if (Vector3.Distance(playerController.transform.position, raycast.point) < Vector3.Distance(playerController.transform.position, targetDirection * range))
            {
                targetUI.GetComponent<SpriteRenderer>().color = Color.red;
                targetValid = false;
                return;
            }
        }
        targetUI.GetComponent<SpriteRenderer>().color = Color.green;

        targetValid = true;
    }
}
