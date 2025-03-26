using UnityEngine;

public class SceneTransition : MonoBehaviour
{
    private GameObject player;

    bool playerInRange;
    private void Awake()
    {
        player = GameObject.Find("Player");
        playerInRange = false;
    }

    private void Update()
    {
        
    }

    private void Interact()
    {
        if (!playerInRange)
        {
            return;
        }
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
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
