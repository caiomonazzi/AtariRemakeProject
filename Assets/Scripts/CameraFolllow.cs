using UnityEngine;

namespace Berzerk
{
    public class CameraFollow : MonoBehaviour
    {
        [Header("Set in Inspector")]
        public Transform player; // Reference to the player's transform
        public float smoothing = 5f; // Smoothing factor for the camera movement

        private Vector3 offset; // Offset from the player

        private void Start()
        {
            // Calculate and store the offset value by getting the distance between the player's position and camera's position.
            offset = transform.position - player.position;
        }

        private void LateUpdate()
        {
            // Create a position the camera is aiming for based on the offset from the player.
            Vector3 targetCamPos = player.position + offset;

            // Ensure the Z axis is always -10
            targetCamPos.z = -10;

            // Smoothly interpolate between the camera's current position and its target position.
            transform.position = Vector3.Lerp(transform.position, targetCamPos, smoothing * Time.deltaTime);
        }
    }
}
