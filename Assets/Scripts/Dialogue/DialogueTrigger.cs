using TMPro;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [Header("Visual Cue")]
    [SerializeField] private GameObject visualCue;
    [SerializeField] private GameObject prompt;

    [Header("Text Asset")]
    [SerializeField] private TextAsset inkJSON;
    bool playerInRange;
    bool activated = false;
    private void Awake()
    {
        playerInRange = false;
        visualCue.SetActive(false);
    }

    private void Update()
    {
        if(DialogueManager.Instance.dialogueIsPlaying)
        {
            return;
        }
        if(activated)
        {
            visualCue.SetActive(false);
            prompt.SetActive(false);
            return;
        }
        if (playerInRange)
        {
            visualCue.SetActive(false);
            prompt.SetActive(true);
        }
        else
        {
            visualCue.SetActive(true);
            prompt.SetActive(false);
        }

        if (GameObject.FindGameObjectsWithTag("Enemy").Length > 0)
        {
            prompt.GetComponentInChildren<TextMeshProUGUI>().text = "Clear Remaining Enemies";
        }
        else
        {
            prompt.GetComponent<TextMeshProUGUI>().text = "Press " + InputManager.GetBindingName("Interact", 0) + " to Listen";
        }
    }

    private void Interact()
    {
        if(activated) { return; }
        if(playerInRange)
        { 
            prompt.SetActive(false);
            DialogueManager.Instance.EnterDialogueMode(inkJSON);
            activated = true;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(playerInRange) { return;}
        if (collision.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    private void OnEnable()
    {
        InputManager.onInteract += Interact;
    }

    private void OnDestroy()
    {
        InputManager.onInteract -= Interact;
    }
}
