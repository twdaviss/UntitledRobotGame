using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraZone : MonoBehaviour
{
    private CinemachineVirtualCamera zoneCamera;
    // Start is called before the first frame update

    private void Awake()
    {
        zoneCamera = GetComponent<CinemachineVirtualCamera>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            zoneCamera.enabled = true;
            Debug.Log("Entering camera zone");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            zoneCamera.enabled = false;
            Debug.Log("Exiting camera zone");
        }
    }
}
