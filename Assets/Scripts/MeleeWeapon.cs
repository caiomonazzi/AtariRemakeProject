using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

namespace Berzerk
{
    public class MeleeWeapon : MonoBehaviour
    {
        public float damage;
        public Animator characterAnimator;
        public CircleCollider2D meleeCollider;
        public float attackCooldown = 0.5f;
        private float lastAttackTime;

        private void Start()
        {
            meleeCollider.enabled = false;
        }

        public void MeeleeAttack()
        {
            if (Time.time >= lastAttackTime + attackCooldown)
            {
                characterAnimator.SetTrigger("meleeAttack");

                StartCoroutine(EnableMeleeCollider());

                lastAttackTime = Time.time;

            }
        }

        private IEnumerator EnableMeleeCollider()
        {
            meleeCollider.enabled = true;
            yield return new WaitForSeconds(0.1f); // Adjust duration as needed
            meleeCollider.enabled = false;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Zombie"))
            {
                ZombieHealth zombieHealth = other.GetComponent<ZombieHealth>();
                Zombie zombie = other.GetComponent<Zombie>();

                if (zombie != null)
                {
                    zombie.Stagger(attackCooldown); // Pass the duration to Stagger method
                    Debug.Log("Zombie staggered.");
                }

                if (zombieHealth != null)
                {
                    zombieHealth.DecreaseHealth(damage);
                    Debug.Log($"Zombie hit! Damage dealt: {damage}. Zombie health: {zombieHealth.health}");
                }
            }
        }
    }
}
