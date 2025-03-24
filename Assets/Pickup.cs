using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PickupType
{
    Ricochet,
    KnockBack,
    Ammo,
    GrapplePull,
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
                player.GetComponentInChildren<ScrapShot>().canRicochet = true;
                break;
            case PickupType.KnockBack:
                player.GetComponentInChildren<Melee>().IncreaseKnockBack(1.5f);
                break;
            case PickupType.Ammo:
                player.GetComponentInChildren<ScrapShot>().maxAmmo += 1;
                player.GetComponentInChildren<ScrapShot>().currentAmmo += 1;
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
