using Berzerk;
using UnityEngine;
using UnityEngine.UI;

public class InteractionCanvas : MonoBehaviour
{

    [Header("Components")]
    public Text InteractionText;

    private bool playerInRange = false;
    private Door currentDoor;

    private void Update()
    {
        // Check if player is in range and the Space key is pressed
        if (playerInRange && Input.GetKeyDown(KeyCode.Space))
        {
            DisplayInteraction();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = true;
            InteractionText.gameObject.SetActive(true); // Show interaction text when player enters the trigger
        }
        else if (collision.CompareTag("Door"))
        {
            currentDoor = collision.GetComponent<Door>();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = false;
            InteractionText.gameObject.SetActive(false); // Hide interaction text when player exits the trigger
        }
    }

    private void DisplayInteraction()
    {
        // Add the logic for what should happen when the player presses the Space key while in range
        Debug.Log("Interaction happened");
    }
}