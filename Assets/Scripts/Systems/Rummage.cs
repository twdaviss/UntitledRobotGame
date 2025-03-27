using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Rummage : MonoBehaviour
{
    [SerializeField] private GameObject item;
    [SerializeField] private GameObject visualCue;
    [SerializeField] private GameObject prompt;


    bool playerInRange;
    bool activated = false;
    private void Awake()
    {
        playerInRange = false;
        visualCue.SetActive(true);
    }
    private void Update()
    {
        if(activated)
        {
            visualCue.SetActive(false);
            prompt.SetActive(false);
        }
        else if (playerInRange)
        {
            visualCue.SetActive(false);
            prompt.SetActive(true);
            prompt.GetComponent<TextMeshProUGUI>().text = "Press " + InputManager.GetBindingName("Interact", 0) + " to Rummage";
            return;
        }
        else
        {
            prompt.SetActive(false);
            visualCue.SetActive(true);
        }
    }

    private void Interact()
    {
        if (activated) { return; }
        if (playerInRange)
        {
            activated = true;
            
            Vector2 spawnPos = new Vector2(transform.position.x, transform.position.y - 2);
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
