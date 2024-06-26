using UnityEngine;

public class InteractionTrigger : MonoBehaviour
{
    private InteractionCanvas canvas;

    [HideInInspector] public bool inTrigger; // Tracking trigger status
    [HideInInspector] public bool isInteracting; // Flag to track interaction status

    private void Start()
    {
        canvas = GetComponentInChildren<InteractionCanvas>(true);
    }

    private void OnTriggerEnter2D(Collider2D collision) // if player ENTER in trigger
    {
        if (collision.gameObject.CompareTag("Player")) // if it's the player
        {
            inTrigger = true;
            canvas.gameObject.SetActive(true); // UI enable
        }
    }

    private void OnTriggerExit2D(Collider2D collision) // if player EXIT in trigger
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            inTrigger = false;
            isInteracting = false; // Reset interaction status
            canvas.gameObject.SetActive(false); // UI disable
        }
    }
}
