using System.Collections;
using UnityEngine;

namespace Berzerk
{
    public class CameraShake : MonoBehaviour
    {
        private Transform cameraTransform;
        private Vector3 originalPosition;

        [Header("Shake Settings")]
        public float shakeDuration = 0.5f;
        public float shakeMagnitude = 0.5f;

        private void Awake()
        {
            cameraTransform = Camera.main.transform;
            originalPosition = cameraTransform.localPosition;
        }

        public void Shake()
        {
            StopAllCoroutines();
            StartCoroutine(ShakeCoroutine());
        }

        private IEnumerator ShakeCoroutine()
        {
            float elapsed = 0.0f;

            while (elapsed < shakeDuration)
            {
                float x = Random.Range(-1f, 1f) * shakeMagnitude;
                float y = Random.Range(-1f, 1f) * shakeMagnitude;

                cameraTransform.localPosition = new Vector3(x, y, originalPosition.z);

                elapsed += Time.deltaTime;

                yield return null;
            }

            cameraTransform.localPosition = originalPosition;
        }
    }
}
