using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Berzerk
{
    public class Bullet : MonoBehaviour
    {
        [Header("Set in Inspector")]
        private BulletPool bulletPool;

        public void Initialize(BulletPool pool)
        {
            bulletPool = pool;
            DeactivateAfterTime(10f);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            // Return the bullet to the pool upon collision
            bulletPool.ReturnBullet(gameObject);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("Zombie"))
            {
                HandleZombieHit(collision.gameObject);
            }
            if (collision.gameObject.CompareTag("Door"))

            {
                bulletPool.ReturnBullet(gameObject);
            }

            if (collision.gameObject.CompareTag("Obstacle"))

            {
                bulletPool.ReturnBullet(gameObject);
            }
        }

        // Handles logic when the bullet hits a zombie
        private void HandleZombieHit(GameObject zombie)
        {
            ZombieHealth zombieHealth = zombie.GetComponent<ZombieHealth>();

            if (zombieHealth != null)
            {
                zombieHealth.DecreaseHealth();
                Destroy(gameObject); // Destroy the bullet
            }
        }

        public void DeactivateAfterTime(float delay)
        {
            StartCoroutine(DeactivateCoroutine(delay));
        }

        private IEnumerator DeactivateCoroutine(float delay)
        {
            yield return new WaitForSeconds(delay);
            bulletPool.ReturnBullet(gameObject);
        }
    }
}
