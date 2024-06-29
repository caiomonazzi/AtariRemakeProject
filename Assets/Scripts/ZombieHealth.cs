using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Berzerk
{
    public class ZombieHealth : MonoBehaviour
    {
        private XPSystem xp;
        private EffectPool effectPool;

        [Header("Assign in inspector")]
        public float health;
        public float damage;

        private void Start()
        {
            // Find the player and get the xp script when it spawns
            xp = GameObject.FindGameObjectWithTag("Player").GetComponent<XPSystem>();

            // Find the effect pool
            effectPool = GameObject.FindGameObjectWithTag("EffectPool").GetComponent<EffectPool>();
        }

        public void Update()
        {
            // If its health is 0 add the blood splat prefab and destroy it
            if (health <= 0)
            {
                GameObject effect = effectPool.GetEffect();
                effect.transform.position = transform.position;
                effect.transform.rotation = transform.rotation;

                xp.IncreaseXP();
                Destroy(this.gameObject);
                StartCoroutine(ReturnEffectToPool(effect, 1f)); // Return effect to pool after 1 second
            }
        }

        private IEnumerator ReturnEffectToPool(GameObject effect, float delay)
        {
            yield return new WaitForSeconds(delay);
            effectPool.ReturnEffect(effect);
        }

        // Take away health if shot
        public void DecreaseHealth()
        {
            health -= damage;
        }
    }
}
