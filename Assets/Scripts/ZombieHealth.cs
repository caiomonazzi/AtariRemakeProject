using System.Collections;
using UnityEngine;

namespace Berzerk
{
    public class ZombieHealth : MonoBehaviour
    {
        private XPSystem xp;
        private EffectPool effectPool;
        private Zombie zombie;

        [Header("Assign in inspector")]
        public float health;
        public float staggerDuration = 0.5f;

        private void Start()
        {
            xp = GameObject.FindGameObjectWithTag("Player").GetComponent<XPSystem>();
            effectPool = GameObject.FindGameObjectWithTag("EffectPool").GetComponent<EffectPool>();
            zombie = GetComponent<Zombie>();
        }

        public void DecreaseHealth(float damage)
        {
            health -= damage;
            if (health <= 0)
            {
                HandleDeath();
            }
            else
            {
                Stagger();
            }
        }

        private void HandleDeath()
        {
            GameObject effect = effectPool.GetEffect();
            effect.transform.position = transform.position;
            xp.IncreaseXP();
            Destroy(gameObject);
            StartCoroutine(ReturnEffectToPool(effect, 1f));
        }

        private void Stagger()
        {
            if (zombie != null)
            {
                zombie.Stagger(staggerDuration);
            }
        }

        private IEnumerator ReturnEffectToPool(GameObject effect, float delay)
        {
            yield return new WaitForSeconds(delay);
            effectPool.ReturnEffect(effect);
        }
    }
}
