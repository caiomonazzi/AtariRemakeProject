using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Berzerk
{
    public class Bullet : MonoBehaviour
    {
        [Header("Set in Inspector")]
        public GameObject bloodEffect; // Effect to instantiate upon hitting a zombie
        private BulletPool bulletPool;

        public void Initialize(BulletPool pool)
        {
            bulletPool = pool;
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
            else
            {
                // Return the bullet to the pool if it hits anything else
                bulletPool.ReturnBullet(gameObject);
            }
        }

        // Handles logic when the bullet hits a zombie
        private void HandleZombieHit(GameObject zombie)
        {
            ZombieHealth zombieHealth = zombie.GetComponent<ZombieHealth>();

            if (zombieHealth != null)
            {
                InstantiateBloodEffect();
                zombieHealth.DecreaseHealth();
                Destroy(gameObject); // Destroy the bullet
            }
        }

        // Instantiates the blood effect at the bullet's position
        private void InstantiateBloodEffect()
        {
            Instantiate(bloodEffect, transform.position, transform.rotation);
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
