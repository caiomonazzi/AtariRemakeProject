using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Berzerk
{
    public class Door : MonoBehaviour
    {
        private SpriteRenderer spriteRenderer;
        private Animator doorAnimator;
        private bool isOpen;
        private Collider2D[] doorColliders;
        private bool isLocked = true;
        private InteractionTrigger interactionTrigger;

        [Header("Settings")]
        public int needKeyID; // Need key ID

        private void Start()
        {
            interactionTrigger = GetComponentInChildren<InteractionTrigger>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            doorColliders = GetComponents<Collider2D>();
            doorAnimator = GetComponent<Animator>();
        }

        private void Update()
        {
            if (interactionTrigger.inTrigger && Input.GetKeyDown(KeyCode.Space))
            {
                interactionTrigger.isInteracting = true;
                if (isLocked)
                {
                    if (Player.Instance.HasKey(needKeyID)) // Key ownership check
                    {
                        Debug.Log("Opening door with key ID: " + needKeyID);
                        OpenDoor();
                    }
                    else
                    {
                        Debug.Log("Player does not have the required key.");
                    }
                }
                else
                {
                    Debug.Log("The door is already unlocked.");
                }
            }
        }

        public void OpenDoor()
        {
            doorAnimator.SetBool("isOpen", true); // Set animator parameter to trigger open animation
            isLocked = false; // Unlock door
            Player.Instance.UseKey(needKeyID); // Remove key from player

            for (int i = 0; i < doorColliders.Length; i++) // Disable colliders
            {
                doorColliders[i].enabled = false;
                gameObject.SetActive(false);
            }
            interactionTrigger.isInteracting = false; // Reset interaction status
            Debug.Log("Door opened with key ID: " + needKeyID);
            Player.Instance.PlayDoorOpenSound(); // Play the door open sound

        }

    }
}
