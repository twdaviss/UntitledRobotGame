using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class RebindUI : MonoBehaviour
{
    [SerializeField] private InputActionReference inputActionReference; //from scriptable object
    [SerializeField] private bool excludeMouse = true;
    
    [Range(0, 10)]
    [SerializeField] private int selectedBinding;
    [SerializeField] private InputBinding.DisplayStringOptions displayStringOptions;
    
    [Header("Binding Info - DO NOT EDIT")]
    [SerializeField] private InputBinding inputBinding;
    private int bindingIndex;
    private string actionName;

    [Header("UI Fields")]
    [SerializeField] private TextMeshProUGUI actionText;
    [SerializeField] private Button rebindButton;
    [SerializeField] private TextMeshProUGUI rebindText;
    [SerializeField] private Button resetButton;

    [SerializeField] private GameObject rebindOverlay;
    [SerializeField] private TextMeshProUGUI rebindOverlayText;

    private bool compositeRebinding;
    private string compositeKeyName;
    private void OnEnable()
    {
        rebindButton.onClick.AddListener(() => DoRebind()); 
        resetButton.onClick.AddListener(() => ResetBinding());

        if(inputActionReference != null)
        {
            LoadBinding();
        }
        InputManager.rebindStarted += EnableOverlay;
        InputManager.disableOverlay += DisableOverlay;
        InputManager.rebindCanceled += UpdateUI;
        InputManager.rebindCompleted += UpdateUI;
        InputManager.compositeBeingRebound += SetIfCompositeAndName;
    }

    private void OnDisable()
    {
        InputManager.rebindStarted -= EnableOverlay;
        InputManager.disableOverlay -= DisableOverlay;
        InputManager.rebindCanceled -= UpdateUI;
        InputManager.rebindCompleted -= UpdateUI;
        InputManager.compositeBeingRebound -= SetIfCompositeAndName;
    }

    private void OnDestroy()
    {
        InputManager.rebindStarted -= EnableOverlay;
        InputManager.disableOverlay -= DisableOverlay;
        InputManager.rebindCanceled -= UpdateUI;
        InputManager.rebindCompleted -= UpdateUI;
        InputManager.compositeBeingRebound -= SetIfCompositeAndName;
    }
    private void OnValidate()
    {
        if(inputActionReference == null)
        {
            return;
        }
        GetBindingInfo();
        UpdateUI();
    }

    private void GetBindingInfo()
    {
        if(inputActionReference.action != null)
        {
            actionName = inputActionReference.action.name;
        }

        if(inputActionReference.action.bindings.Count > selectedBinding)
        {
            inputBinding = inputActionReference.action.bindings[selectedBinding];
            bindingIndex = selectedBinding;
        }
    }

    private void UpdateUI()
    {
        if (actionText != null)
        {
            actionText.text = actionName;
        }
        if (rebindText != null)
        {
            if (Application.isPlaying)
            {
                if (compositeRebinding)
                {
                    rebindText.text = compositeKeyName;
                }
                else
                {
                    rebindText.text = InputManager.GetBindingName(actionName, bindingIndex);
                }
            }
            else
            {
                rebindText.text = inputActionReference.action.GetBindingDisplayString(bindingIndex);
            }
        }
    }

    private void EnableOverlay(InputAction actionToRebind, int binding)
    {
        rebindOverlay?.SetActive(true);
        rebindOverlayText.text = "Press a " + actionToRebind.expectedControlType + " to Rebind";
    }

    private void DisableOverlay()
    {
        rebindOverlay?.SetActive(false);
    }

    private void DoRebind()
    {
        InputManager.StartRebind(actionName, bindingIndex, rebindText, excludeMouse);
    }

    public void SetIfCompositeAndName(bool isComposite, string name, string actionName)
    {
        if (actionName == this.actionName)
        {
            compositeRebinding = isComposite;
            compositeKeyName = name;
        }
        else compositeRebinding = false;
    }

    private void LoadBinding()
    {
        InputManager.LoadBindingOverride(actionName);
        GetBindingInfo();
        UpdateUI();
    }
    private void ResetBinding()
    {
        InputManager.ResetBinding(actionName, bindingIndex);
        UpdateUI();
    }
}
