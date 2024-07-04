using UnityEngine;

namespace Berzerk
{
    public class CameraFollow : MonoBehaviour
    {
        private Transform player;
        private Vector3 offset;
        private Camera mainCamera;

        private void Start()
        {
            mainCamera = GetComponent<Camera>();
            FindPlayer();
            if (player != null)
            {
                SetCameraPosition(); // Immediately set the camera position to follow the player
            }
        }

        private void LateUpdate()
        {
            if (player == null)
            {
                FindPlayer();
            }

            if (player != null)
            {
                SetCameraPosition(); // Continuously ensure the camera follows the player
            }
        }

        public void FindPlayer()
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                Debug.Log("Player found");
                player = playerObj.transform;
                offset = transform.position - player.position; // Recalculate offset if the player is found
                SetCameraPosition(); // Immediately set the camera position to follow the player
            }
        }

        private void SetCameraPosition()
        {
            if (player != null)
            {
                Vector3 targetCamPos = player.position + offset;
                targetCamPos.z = -15; // Ensure the camera's Z position remains fixed
                transform.position = targetCamPos;
            }
        }
    }
}
