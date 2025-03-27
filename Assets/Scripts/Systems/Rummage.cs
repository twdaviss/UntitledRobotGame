using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rummage : MonoBehaviour
{
    [Header("Visual Cue")]
    [SerializeField] private GameObject visualCue;
    [SerializeField] private GameObject item;

    bool playerInRange;
    bool activated = false;
    private void Awake()
    {
        playerInRange = false;
        visualCue.SetActive(true);
    }

    private void Interact()
    {
        if (activated) { return; }
        if (playerInRange)
        {
            activated = true;
            int randX = Random.Range(2, 5);
            int randY = Random.Range(2, 5);

            Vector2 spawnPos = new Vector2(transform.position.x + randX, transform.position.y + randY);
            GameObject itemObject = Instantiate(item, spawnPos, item.transform.rotation);
            visualCue.SetActive(false);
        }
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
