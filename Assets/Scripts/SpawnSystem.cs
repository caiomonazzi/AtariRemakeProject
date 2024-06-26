using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Berzerk
{
    public class SpawnSystem : MonoBehaviour
    {
        [Header("Spawn Settings")]
        public GameObject[] zombies;
        public float waitTime;

        private void Start()
        {
            StartCoroutine(SpawnCoroutine());
        }

        private IEnumerator SpawnCoroutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(waitTime);
                SpawnZombie();
            }
        }

        private void SpawnZombie()
        {
            int rand = Random.Range(0, zombies.Length);
            Instantiate(zombies[rand], transform.position, transform.rotation);
        }
    }
}
