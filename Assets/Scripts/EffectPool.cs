using System.Collections.Generic;
using UnityEngine;

namespace Berzerk
{
    public class EffectPool : MonoBehaviour
    {
        public GameObject effectPrefab;
        public int poolSize = 10;

        private Queue<GameObject> effectPool;

        private void Awake()
        {
            effectPool = new Queue<GameObject>();

            for (int i = 0; i < poolSize; i++)
            {
                GameObject effect = Instantiate(effectPrefab, transform);
                effect.SetActive(false);
                effectPool.Enqueue(effect);
            }
        }

        public GameObject GetEffect()
        {
            if (effectPool.Count > 0)
            {
                GameObject effect = effectPool.Dequeue();
                effect.SetActive(true);
                return effect;
            }
            else
            {
                GameObject effect = Instantiate(effectPrefab, transform);
                effect.SetActive(false);
                return effect;
            }
        }

        public void ReturnEffect(GameObject effect)
        {
            effect.SetActive(false);
            effect.transform.SetParent(transform); // Re-parent the effect back to the pool
            effectPool.Enqueue(effect);
        }
    }
}
