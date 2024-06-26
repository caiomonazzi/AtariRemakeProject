using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Berzerk
{
    public class NextLevelDoor : MonoBehaviour
    {
        private BoxCollider2D boxCollider2D;

        [Header("Components")]
        private SpriteRenderer spriteRenderer;
        public bool lockedDoor; // Door status

        private bool inTrigger;
        private InteractionTrigger interactionTrigger;

        private void Start()
        {
            interactionTrigger = GetComponentInChildren<InteractionTrigger>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            boxCollider2D = GetComponent<BoxCollider2D>();

            CheckLockStatus(); // Check door status
        }

        private void Update()
        {
            // if player is in trigger
            if (interactionTrigger.inTrigger)
            {
                // if player presses Interaction button
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    if (!lockedDoor) // if door is unlocked
                    {
                        interactionTrigger.isInteracting = false;
                        GoToNextLevel(); // go to next level
                    }
                }
            }
        }

        // Check door status method
        public void CheckLockStatus()
        {
            if (lockedDoor) // if door is locked
            {
                interactionTrigger.isInteracting = false;
                boxCollider2D.enabled = false; // disable collider
            }
            else
            {
                interactionTrigger.isInteracting = false;
                boxCollider2D.enabled = true; // enable collider
            }
        }

        // Next level method
        private void GoToNextLevel()
        {
            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
            int nextSceneIndex = currentSceneIndex + 1;

            if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
            {
                StartCoroutine(LoadNextScene(nextSceneIndex));
            }
            else
            {
                Debug.Log("No more levels to load!");
            }
        }

        private IEnumerator LoadNextScene(int sceneIndex)
        {
            // Assuming you have a transition animation, you can play it here
            yield return SceneManager.LoadSceneAsync(sceneIndex);

            // Find the new spawn point in the new scene
            GameObject newSpawnPoint = GameObject.FindGameObjectWithTag("SpawnPoint");

            if (newSpawnPoint != null)
            {
                // Find the player instance that is carried through levels
                GameObject player = GameObject.FindWithTag("Player");

                if (player != null)
                {
                    // Move the player to the new spawn point position
                    player.transform.position = newSpawnPoint.transform.position;

                    // Destroy the old player object if necessary
                    DestroyImmediate(newSpawnPoint);
                }
            }
        }
    }
}
