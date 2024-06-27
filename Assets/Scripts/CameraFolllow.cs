using UnityEngine;

namespace Berzerk
{
    public class CameraFollow : MonoBehaviour
    {
        public float smoothing = 5f;
        private Transform player;
        private Vector3 offset;

        private void Start()
        {
            FindPlayer();
        }

        private void LateUpdate()
        {
            if (player == null)
            {
                FindPlayer();
            }

            if (player != null)
            {
                Vector3 targetCamPos = player.position + offset;
                targetCamPos.z = -10;
                transform.position = Vector3.Lerp(transform.position, targetCamPos, smoothing * Time.deltaTime);
            }
        }

        private void FindPlayer()
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
                offset = transform.position - player.position;
            }
        }
    }
}
