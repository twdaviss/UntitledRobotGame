using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    [SerializeField] private PlayerControls playerControls;
    private PlayerController playerController;
    private Vector3 mouseScreenPosition;

    void Awake()
    {
        playerController = GetComponent<PlayerController>();
        playerControls = new PlayerControls();
        playerControls.Menu.Disable();
        playerControls.Gameplay.Enable();
    }

    public void PauseGame(InputAction.CallbackContext context)
    {
        if (!gameObject.scene.IsValid())
        {
            return;
        }
        if (GameManager.Instance.IsPauseMenuEnabled())
        {
            playerControls.Menu.Disable();
            playerControls.Gameplay.Enable();
            GameManager.Instance.DisablePauseMenu();
        }
        else
        {
            playerControls.Menu.Enable();
            playerControls.Gameplay.Disable();
            GameManager.Instance.EnablePauseMenu();
        }
    }

    public void UnPauseGame()
    {
        playerControls.Menu.Disable();
        playerControls.Gameplay.Enable();
    }

    public Vector2 GetMouseDirection()
    {
        Vector3 mouseDirection = (Vector2)playerController.activeCamera.ScreenToWorldPoint(mouseScreenPosition) - (Vector2)transform.position;
        return mouseDirection.normalized;
    }

    public Vector3 GetMousePosition()
    {
        Vector3 mouseWorldPosition = (Vector2)playerController.activeCamera.ScreenToWorldPoint(mouseScreenPosition);
        return mouseWorldPosition;
    }

    public float GetMouseDistance()
    {
        return Vector3.Distance(transform.position, GetMousePosition());
    }

    private void OnEnable()
    {
        playerControls.Gameplay.Enable();

        playerControls.Gameplay.Sprint.performed += ctx => {
            playerController.isSprinting = !playerController.isSprinting;
        };
        GameManager.onUnPaused += UnPauseGame;
        playerControls.Gameplay.Pause.performed += PauseGame;
        playerControls.Menu.Pause.performed += PauseGame;

        playerControls.Gameplay.Move.performed += ctx => playerController.moveDirection = ctx.ReadValue<Vector2>();
        playerControls.Gameplay.Mouse.performed += ctx => mouseScreenPosition = ctx.ReadValue<Vector2>();
        playerControls.Gameplay.Move.canceled += ctx => playerController.moveDirection = Vector2.zero;
        playerControls.Gameplay.Fire.performed += ctx =>
        {
            playerController.scrapShot.ShootScrap();
        };
        playerControls.Gameplay.Magnetize.performed += ctx =>
        {
            playerController.scrapShot.MagnetizeScrap();
        };
        playerControls.Gameplay.Melee.performed += ctx => playerController.melee.Attack();
        playerControls.Gameplay.Sling.performed += ctx => playerController.grapple.GrappleStart();
        playerControls.Gameplay.Sling.performed += ctx => playerController.lookingForTarget = true;

        playerControls.Gameplay.Sling.canceled += ctx => playerController.grapple.GrappleReleased();
        playerControls.Gameplay.Sling.canceled += ctx => playerController.StopLookingForSlingTarget();
    }

    private void OnDestroy()
    {
        GameManager.onUnPaused -= UnPauseGame;

        playerControls.Gameplay.Sprint.performed -= ctx => {
            playerController.isSprinting = !playerController.isSprinting;
        };
        playerControls.Gameplay.Pause.performed -= PauseGame;
        playerControls.Menu.Pause.performed -= PauseGame;
        GameManager.onUnPaused -= UnPauseGame;

        playerControls.Gameplay.Move.performed -= ctx => playerController.moveDirection = ctx.ReadValue<Vector2>();
        playerControls.Gameplay.Mouse.performed -= ctx => mouseScreenPosition = ctx.ReadValue<Vector2>();
        playerControls.Gameplay.Move.canceled -= ctx => playerController.moveDirection = Vector2.zero;
        playerControls.Gameplay.Fire.performed -= ctx =>
        {
            playerController.scrapShot.ShootScrap();
        };
        playerControls.Gameplay.Magnetize.performed -= ctx =>
        {
            playerController.scrapShot.MagnetizeScrap();
        };
        playerControls.Gameplay.Melee.performed -= ctx => playerController.melee.Attack();
        playerControls.Gameplay.Sling.performed -= ctx => playerController.grapple.GrappleStart();
        playerControls.Gameplay.Sling.performed -= ctx => playerController.lookingForTarget = true;

        playerControls.Gameplay.Sling.canceled -= ctx => playerController.grapple.GrappleReleased();
        playerControls.Gameplay.Sling.canceled -= ctx => playerController.StopLookingForSlingTarget();
    }
}
