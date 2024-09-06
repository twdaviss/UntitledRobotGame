using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System;
using TMPro;
using System.Linq;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    public static PlayerControls playerControls;

    public static event Action rebindCompleted;
    public static event Action rebindCanceled;
    public static event Action enableOverlay;
    public static event Action disableOverlay;
    public static event Action<InputAction, int> rebindStarted;
    public static event Action<bool, string, string> compositeBeingRebound;

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
    private void Start()
    {
        playerControls.Menu.Disable();
        playerControls.Gameplay.Enable();
    }

    public void PauseGame(InputAction.CallbackContext context)
    {
        if (GameManager.Instance.IsOptionsMenuEnabled())
        {
            GameManager.Instance.DisableOptionsMenu();
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

    public static void StartRebind(string actionName, int bindingIndex, TextMeshProUGUI statusText, bool excludeMouse)
    {
        InputAction action = playerControls.asset.FindAction(actionName);
        if(action == null || action.bindings.Count <= bindingIndex)
        {
            Debug.Log("Couldn't find action or binding");
            return;
        }

        if (action.bindings[bindingIndex].isComposite)
        {
            var firstPartIndex = bindingIndex + 1;
            if(firstPartIndex < action.bindings.Count && action.bindings[firstPartIndex].isPartOfComposite)
            {
                DoRebind(action, firstPartIndex, statusText, true, excludeMouse);
            }
        }
        else
        {
            DoRebind(action, bindingIndex, statusText, false, excludeMouse);
        }
    }

    private static void DoRebind(InputAction actionToRebind, int bindingIndex, TextMeshProUGUI statusText, bool allCompositeParts, bool excludeMouse)
    {
        if(actionToRebind == null || bindingIndex < 0)
        {
            return;
        }

        if (actionToRebind.bindings[bindingIndex].isPartOfComposite)
        {
            statusText.text = $"Binding '{actionToRebind.bindings[bindingIndex].name}'. ";
            compositeBeingRebound?.Invoke(true, statusText.text, actionToRebind.name);

        }
        else
        {
            statusText.text = "Press a " + actionToRebind.expectedControlType;
            compositeBeingRebound?.Invoke(false, "", actionToRebind.name);
        }

        actionToRebind.Disable();

        var rebind = actionToRebind.PerformInteractiveRebinding(bindingIndex);
        rebind.OnComplete(operation =>
        {
            actionToRebind.Enable();
            operation.Dispose();
            disableOverlay?.Invoke();
            if (allCompositeParts)
            {
                var nextBindingIndex = bindingIndex + 1;
                if (nextBindingIndex < actionToRebind.bindings.Count && actionToRebind.bindings[nextBindingIndex].isPartOfComposite)
                { 
                    DoRebind(actionToRebind, nextBindingIndex, statusText, allCompositeParts, excludeMouse); 
                }
                else
                {
                    compositeBeingRebound?.Invoke(false, "", actionToRebind.name);
                }
            }

            SaveBindingOverride(actionToRebind);
            rebindCompleted?.Invoke();
        });

        rebind.OnCancel(operation =>
        {
            //actionToRebind.Enable();
            operation.Dispose();
            disableOverlay?.Invoke();

            rebindCanceled?.Invoke();
        });

        rebind.WithCancelingThrough("<Keyboard>/escape");

        if (excludeMouse)
        {
            rebind.WithControlsExcluding("Mouse");
        }

        rebindStarted?.Invoke(actionToRebind, bindingIndex);
        rebind.Start(); //actually starts the rebinding
    }

    public static string GetBindingName(string actionName, int bindingIndex)
    {
        if(playerControls == null)
        {
            playerControls = new PlayerControls();
        }

        InputAction action = playerControls.asset.FindAction(actionName);
        return action.GetBindingDisplayString(bindingIndex);
    }

    private static void SaveBindingOverride(InputAction action)
    {
        for(int i = 0; i < action.bindings.Count; i++)
        {
            PlayerPrefs.SetString(action.actionMap + action.name + i, action.bindings[i].overridePath);
        }
    }

    public static void LoadBindingOverride(string actionName)
    {
        if (playerControls == null)
        {
            playerControls = new PlayerControls();
        }

        InputAction action = playerControls.asset.FindAction(actionName);

        for(int i = 0; i < action.bindings.Count; i++)
        {
            string path = PlayerPrefs.GetString(action.actionMap + action.name + i);

            if (!string.IsNullOrEmpty(path))
            {
                action.ApplyBindingOverride(i, path);
            }
        }
    }

    public static void ResetBinding(string actionName, int bindingIndex)
    {
        InputAction action = playerControls.asset.FindAction(actionName);

        if(action == null || action.bindings.Count <= bindingIndex)
        {
            Debug.Log("Could not find action or binding");
            return;
        }

        if (action.bindings[bindingIndex].isComposite)
        {
            for (int i = bindingIndex + 1; i < action.bindings.Count && action.bindings[i].isPartOfComposite; i++)
            {
                action.RemoveBindingOverride(i);
            }
        }
        else
        {
            action.RemoveBindingOverride(bindingIndex);
        }

        SaveBindingOverride(action);
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
        playerControls.Gameplay.Scrap.performed += ctx => ScrapShot();
        playerControls.Gameplay.Magnetize.performed += ctx => Magnetize();
        playerControls.Gameplay.Melee.performed += ctx => Melee();
        playerControls.Gameplay.Grapple.performed += ctx => GrappleStart();
        playerControls.Gameplay.Grapple.canceled += ctx => GrappleStop();
    }

    private void OnDestroy()
    {
        GameManager.onUnPaused -= UnPauseGame;
        playerControls.Gameplay.Pause.performed -= PauseGame;
        playerControls.Menu.Pause.performed -= PauseGame;

        playerControls.Gameplay.Mouse.performed -= ctx => mouseScreenPosition = ctx.ReadValue<Vector2>();
        playerControls.Gameplay.Move.performed -= ctx => moveDirection = ctx.ReadValue<Vector2>();
        playerControls.Gameplay.Sprint.performed -= ctx => Sprint();
        playerControls.Gameplay.Scrap.performed -= ctx => ScrapShot();
        playerControls.Gameplay.Magnetize.performed -= ctx => Magnetize();
        playerControls.Gameplay.Melee.performed -= ctx => Melee();
        playerControls.Gameplay.Grapple.performed -= ctx => GrappleStart();
        playerControls.Gameplay.Grapple.canceled -= ctx => GrappleStop();
    }
}