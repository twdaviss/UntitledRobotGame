using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [Header("Visual Cue")]
    [SerializeField] private GameObject visualCue;
    [SerializeField] private GameObject prompt;

    [Header("Text Asset")]
    [SerializeField] private TextAsset inkJSON;
    bool playerInRange;
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
    }

    private void Interact()
    {
        if(playerInRange)
        {
            prompt.SetActive(false);
            DialogueManager.Instance.EnterDialogueMode(inkJSON);
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
