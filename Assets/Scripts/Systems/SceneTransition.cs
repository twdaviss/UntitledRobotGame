using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    [SerializeField] private GameScene scene;
    [SerializeField] private GameObject prompt;

    bool playerInRange;
    private void Awake()
    {
        playerInRange = false;
    }

    private void Update()
    {
        if (GameObject.FindGameObjectsWithTag("Enemy").Length > 0)
        {
            prompt.GetComponentInChildren<TextMeshProUGUI>().text = "Clear Remaining Enemies";
        }
        else if (!playerInRange)
        {
            prompt.GetComponentInChildren<TextMeshProUGUI>().text = "Come here to Leave Level";
        }
        else
        {
            prompt.GetComponentInChildren<TextMeshProUGUI>().text = "Press " + InputManager.GetBindingName("Interact", 0) + " for Next Level";
        }
    }
    private void Interact()
    {
        if (!playerInRange)
        {
            return;
        }
        if(GameObject.FindGameObjectsWithTag("Enemy").Length <= 0)
        {
            SceneManager.LoadScene(scene.ToString().Prettify());
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
