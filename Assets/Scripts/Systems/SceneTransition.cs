using UnityEngine;
using UnityEngine.SceneManagement;

public enum Scenes
{
    Building,
    Alley,
    Street,
    Junkyard,
    ParkingLot,
}
public class SceneTransition : MonoBehaviour
{
    [SerializeField] private Scenes scene;

    bool playerInRange;
    private void Awake()
    {
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
        switch (scene)
        {
            case Scenes.Building:
                SceneManager.LoadScene("Scene5Building");
                break;
            case Scenes.Alley:
                SceneManager.LoadScene("Scene3Alley");
                break;
            case Scenes.Street:
                SceneManager.LoadScene("Scene4Street");
                break;
            case Scenes.Junkyard:
                SceneManager.LoadScene("Scene1Junkyard");
                break;
            case Scenes.ParkingLot:
                SceneManager.LoadScene("Scene2ParkingLot");
                break;
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
