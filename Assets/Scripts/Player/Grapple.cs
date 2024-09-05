using RobotGame.States;
using UnityEngine;

public class Grapple : MonoBehaviour
{
    [SerializeField]private GameObject targetUI;
    [SerializeField] float range;
    [SerializeField] float speed;
    

    private LineRenderer lineRenderer;
    private PlayerController playerController;
    private GameObject grappleTarget;

    private Vector3 endPosition;
    private bool isAimingGrapple = false;
    private bool targetValid = false;

    private float grappleAimMaxTime = 0.5f;
    private float grapplAimTimer = 0.0f;
    private void Awake()
    {
        playerController = GetComponentInParent<PlayerController>();
        lineRenderer = GetComponentInParent<LineRenderer>();
    }

    private void Update()
    {
        if (!isAimingGrapple || grappleTarget == null)
        {
            targetUI.SetActive(false);
            lineRenderer.enabled = false;
            return;
        }

        grapplAimTimer += Time.deltaTime;
        if (grapplAimTimer >= grappleAimMaxTime)
        {
            CancelGrapple();
            grapplAimTimer = 0.0f;
            return;
        }

        CheckGrappleTarget();
        
        targetUI.SetActive(true);
        targetUI.transform.position = grappleTarget.transform.position;

        //lineRenderer.enabled = true;
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, transform.parent.position);
        lineRenderer.SetPosition(1, grappleTarget.transform.position);
    }

    public void GrappleStart()
    {
        if (playerController.grappleCooldownTimer < playerController.grappleCooldownTime) 
        {
            return;
        }
        GameManager.Instance.SetSlowMoTimeScale();
        isAimingGrapple = true;
    }

    public void GrappleReleased()
    {
        if (!isAimingGrapple) { return; }

        StartCoroutine(GameManager.Instance.ResetTimeScale());
        isAimingGrapple = false;
        targetUI.SetActive(false);
        lineRenderer.enabled = false;

        CheckGrappleTarget();

        if (targetValid && playerController.grappleCooldownTimer >= playerController.grappleCooldownTime)
        {
            playerController.SetState(new PlayerGrappling(playerController, grappleTarget.transform.position, speed));
            playerController.grappleCooldownTimer = 0.0f;
        }
    }

    public void CancelGrapple()
    {
        StartCoroutine(GameManager.Instance.ResetTimeScale());
        isAimingGrapple = false;
        targetUI.SetActive(false);
        lineRenderer.enabled = false;
    }

    public void SetTarget(GameObject target)
    {
        grappleTarget = target;
    }

    private void CheckGrappleTarget()
    {
        if(grappleTarget == null) 
        {
            targetValid = false;
            return; 
        }
        float targetDistance = Vector3.Distance(playerController.transform.position, grappleTarget.transform.position);
        if (targetDistance > range || targetDistance < 4.0f)
        {
            targetUI.GetComponent<SpriteRenderer>().color = Color.red;
            targetValid = false;
            return;
        }
        int layerMask = LayerMask.GetMask("Obstacles");
        Vector2 targetDirection = (grappleTarget.transform.position - transform.position).normalized;

        RaycastHit2D raycast = Physics2D.Raycast(playerController.transform.position, targetDirection, range,layerMask);
        if(raycast.point != null && raycast.point != Vector2.zero)
        {
            if (Vector3.Distance(playerController.transform.position,raycast.point) < Vector3.Distance(playerController.transform.position, targetDirection * range))
            {
                targetUI.GetComponent<SpriteRenderer>().color = Color.red;
                targetValid = false;
                return;
            }
        }
        targetUI.GetComponent<SpriteRenderer>().color = Color.green;

        targetValid = true;
        return;
    }
}
