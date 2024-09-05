using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    [SerializeField] private PlayerControls playerControls;
    [SerializeField] private Camera activeCamera;
    private Vector3 mouseScreenPosition;
    private Vector3 moveDirection;

    public delegate void OnScrapShot();
    public static event OnScrapShot onScrapShot;

    public delegate void OnMelee();
    public static event OnMelee onMelee;

    public delegate void OnGrappleStart();
    public static event OnGrappleStart onGrappleStart;

    public delegate void OnGrappleStop();
    public static event OnGrappleStop onGrappleStop;

    public delegate void OnSprint();
    public static event OnSprint onSprint;

    public delegate void OnMagnetize();
    public static event OnMagnetize onMagnetize;
    private void Start()
    {
        playerControls.Menu.Disable();
        playerControls.Gameplay.Enable();
    }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        if (playerControls == null)
        {
            playerControls = new PlayerControls();
        }
    }

    public PlayerControls GetPlayerControls()
    {
        if(playerControls == null)
        {
            playerControls = new PlayerControls();
        }
        return playerControls;
    }

    public void PauseGame(InputAction.CallbackContext context)
    {
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

    public Vector3 GetMousePosition()
    {
        Vector3 mouseWorldPosition = (Vector2)activeCamera.ScreenToWorldPoint(mouseScreenPosition);
        return mouseWorldPosition;
    }

    public Vector2 GetMoveDirection()
    {
        return moveDirection;
    }
    private void Sprint()
    {
        if (onSprint != null)
        {
            onSprint();
        }
    }

    private void ScrapShot()
    {
        if(onScrapShot != null)
        {
            onScrapShot();
        }
    }

    private void GrappleStart()
    {
        if (onGrappleStart != null)
        {
            onGrappleStart();
        }
    }
    private void GrappleStop()
    {
        if (onGrappleStop != null)
        {
            onGrappleStop();
        }
    }
    private void Melee()
    {
        if (onMelee != null)
        {
            onMelee();
        }
    }
    private void Magnetize()
    {
        if (onMagnetize != null)
        {
            onMagnetize();
        }
    }
    private void OnEnable()
    {
        playerControls.Gameplay.Enable();
        GameManager.onUnPaused += UnPauseGame;
        playerControls.Gameplay.Pause.performed += PauseGame;
        playerControls.Menu.Pause.performed += PauseGame;

        playerControls.Gameplay.Mouse.performed += ctx => mouseScreenPosition = ctx.ReadValue<Vector2>();
        playerControls.Gameplay.Move.performed += ctx => moveDirection = ctx.ReadValue<Vector2>();
        playerControls.Gameplay.Sprint.performed += ctx => Sprint();
        playerControls.Gameplay.Fire.performed += ctx => ScrapShot();
        playerControls.Gameplay.Magnetize.performed += ctx => Magnetize();
        playerControls.Gameplay.Melee.performed += ctx => Melee();
        playerControls.Gameplay.Sling.performed += ctx => GrappleStart();
        playerControls.Gameplay.Sling.canceled += ctx => GrappleStop();
    }

    private void OnDestroy()
    {
        GameManager.onUnPaused -= UnPauseGame;
        playerControls.Gameplay.Pause.performed -= PauseGame;
        playerControls.Menu.Pause.performed -= PauseGame;

        playerControls.Gameplay.Mouse.performed -= ctx => mouseScreenPosition = ctx.ReadValue<Vector2>();
        playerControls.Gameplay.Move.performed -= ctx => moveDirection = ctx.ReadValue<Vector2>();
        playerControls.Gameplay.Sprint.performed -= ctx => Sprint();
        playerControls.Gameplay.Fire.performed -= ctx => ScrapShot();
        playerControls.Gameplay.Magnetize.performed -= ctx => Magnetize();
        playerControls.Gameplay.Melee.performed -= ctx => Melee();
        playerControls.Gameplay.Sling.performed -= ctx => GrappleStart();
        playerControls.Gameplay.Sling.canceled -= ctx => GrappleStop();
    }
}
