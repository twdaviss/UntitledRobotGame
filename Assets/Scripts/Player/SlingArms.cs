using RobotGame.States;
using UnityEngine;

public class SlingArms : MonoBehaviour
{
    [SerializeField]private GameObject targetUI;
    [SerializeField] float range;
    [SerializeField] float speed;
    

    private LineRenderer lineRenderer;
    private PlayerController playerController;
    private GameObject slingTarget;

    private Vector3 endPosition;
    private bool isAimingSling = false;
    private bool targetValid = false;
    private void Awake()
    {
        playerController = GetComponentInParent<PlayerController>();
        lineRenderer = GetComponentInParent<LineRenderer>();
    }

    private void Update()
    {
        if (!isAimingSling || slingTarget == null)
        {
            targetUI.SetActive(false);
            lineRenderer.enabled = false;
            return;
        }

        CheckSlingTarget();

        targetUI.SetActive(true);
        targetUI.transform.position = slingTarget.transform.position;

        //lineRenderer.enabled = true;
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, transform.parent.position);
        lineRenderer.SetPosition(1, slingTarget.transform.position);
    }

    public void SlingStart()
    {
        GameManager.Instance.SetSlowMoTimeScale();
        isAimingSling = true;
    }

    public void SlingReleased()
    {
        StartCoroutine(GameManager.Instance.ResetTimeScale());
        isAimingSling = false;
        targetUI.SetActive(false);
        lineRenderer.enabled = false;

        CheckSlingTarget();

        if (targetValid)
        {
            playerController.SetState(new PlayerSlinging(playerController, slingTarget.transform.position, speed));
        }
    }

    public void SetTarget(GameObject target)
    {
        slingTarget = target;
    }

    private void CheckSlingTarget()
    {
        if(slingTarget == null) 
        {
            targetValid = false;
            return; 
        }
        float targetDistance = Vector3.Distance(playerController.transform.position, slingTarget.transform.position);
        if (targetDistance > range || targetDistance < 4.0f)
        {
            targetUI.GetComponent<SpriteRenderer>().color = Color.red;
            targetValid = false;
            return;
        }
        int layerMask = LayerMask.GetMask("Obstacles");
        Vector2 targetDirection = (slingTarget.transform.position - transform.position).normalized;

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
