using TMPro;
using UnityEngine;
public enum PickupType
{
    Ricochet,
    KnockBack,
    GrapplePull,
    Health,
    Ammo,
    AutoHeal,
}

public class Pickup : MonoBehaviour
{
    [SerializeField] private PickupType pickup;
    [SerializeField] private Sprite sprite;
    [SerializeField] private GameObject prompt;
    [SerializeField] private GameObject description;

    GameObject player;
    bool playerInRange;
    private void Awake()
    {
        player = GameObject.Find("Player");
        playerInRange = false;
        GetComponentInChildren<SpriteRenderer>().sprite = sprite;
    }

    private void Update()
    {
        if (!playerInRange)
        {
            prompt.SetActive(false);
            description.SetActive(false);
            return;
        }
        prompt.SetActive(true);
        prompt.GetComponent<TextMeshProUGUI>().text = "Press " + InputManager.GetBindingName("Interact", 0) + " to Pick Up"; 
        description.SetActive(true);
    }

    private void Interact()
    {
        if (!playerInRange)
        {
            return;
        }
        switch (pickup)
        {
            case PickupType.Ricochet:
                GameManager.Instance.enableRicochet = true;
                break;
            case PickupType.KnockBack:
                GameManager.Instance.increaseKnockBack = true;
                break;
            case PickupType.GrapplePull:
                GameManager.Instance.enableGrapplePull = true;
                break;
            case PickupType.Health:
                player.GetComponentInChildren<PlayerHealth>().Heal(20);
                break;
            case PickupType.Ammo:
                player.GetComponentInChildren<ScrapShot>().IncreaseAmmo(1);
                break;
            case PickupType.AutoHeal:
                GameManager.Instance.enableAutoHeal = true;
                break;
        }
        Destroy(gameObject);
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
