using RobotGame.States;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SlingArms : MonoBehaviour
{
    [SerializeField] float range;
    [SerializeField] float speed;

    private LineRenderer lineRenderer;
    private PlayerController playerController;
    private Vector3 endPosition;
    private bool isAimingSling = false;
    private void Awake()
    {
        playerController = GetComponentInParent<PlayerController>();
        lineRenderer = GetComponentInParent<LineRenderer>();
    }

    private void Update()
    {
        if(isAimingSling)
        {
            endPosition = GetSlingEndPoint();
            lineRenderer.enabled = true;
            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(0, transform.parent.position);
            lineRenderer.SetPosition(1, endPosition);
        }
        else
        {
            lineRenderer.enabled = false;
        }
    }
    public void SlingStart()
    {
        isAimingSling = true;
    }

    public void SlingReleased()
    {
        isAimingSling=false;
        playerController.SetState(new PlayerSlinging(playerController,endPosition,speed));
    }

    private Vector3 GetSlingEndPoint()
    {
        int layerMask = LayerMask.GetMask("Obstacles");

        RaycastHit2D raycast = Physics2D.Raycast(playerController.transform.position, playerController.GetMouseDirection(),range,layerMask);
        if(raycast.point != null && raycast.point != Vector2.zero)
        {
            if (Vector3.Distance(playerController.transform.position,raycast.point) < Vector3.Distance(playerController.transform.position, playerController.GetMouseDirection() * range))
            {
                return raycast.point;
            }
        }
        Vector3 clampedMouseDistance = Vector3.ClampMagnitude((playerController.GetMousePosition() - transform.position), (playerController.GetMouseDirection() * range).magnitude);
        Debug.Log(playerController.GetMouseDirection() * range + ", " + playerController.GetMouseDistance());
        Debug.Log("endpoint: " + transform.position + "," + clampedMouseDistance);
        return transform.position + clampedMouseDistance;
    }
}
