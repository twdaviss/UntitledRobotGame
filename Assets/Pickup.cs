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
    private GameObject player;

    bool playerInRange;
    private void Awake()
    {
        player = GameObject.Find("Player");
        playerInRange = false;
        GetComponentInChildren<SpriteRenderer>().sprite = sprite;
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
        switch (pickup)
        {
            case PickupType.Ricochet:
                player.GetComponentInChildren<ScrapShot>().EnableRicochet();
                break;
            case PickupType.KnockBack:
                player.GetComponentInChildren<Melee>().IncreaseKnockBack(1.5f);
                break;
            case PickupType.GrapplePull:
                player.GetComponentInChildren<Grapple>().EnableGrapplePull();
                break;
            case PickupType.Health:
                player.GetComponentInChildren<PlayerHealth>().Heal(20);
                break;
            case PickupType.Ammo:
                player.GetComponentInChildren<ScrapShot>().IncreaseAmmo(1);
                break;
            case PickupType.AutoHeal:
                player.GetComponentInChildren<PlayerHealth>().EnableAutoHeal();
                break;
        }
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
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
