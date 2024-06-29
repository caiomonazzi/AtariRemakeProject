using System.Collections.Generic;
using UnityEngine;

namespace Berzerk
{
    public class ZombieManager : MonoBehaviour
    {
        public static ZombieManager Instance { get; private set; }

        public List<Zombie> zombies = new List<Zombie>();

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
        }

        public void RegisterZombie(Zombie zombie)
        {
            if (zombie == null) return;

            if (!zombies.Contains(zombie))
            {
                zombies.Add(zombie);
            }
        }

        public void UnregisterZombie(Zombie zombie)
        {
            if (zombie == null) return;

            if (zombies.Contains(zombie))
            {
                zombies.Remove(zombie);
            }
        }

        public void OnGunShot(Vector2 shotPosition, float hearingRadius)
        {
            foreach (var zombie in zombies)
            {
                if (zombie == null) continue;

                float distance = Vector2.Distance(shotPosition, zombie.transform.position);
                if (distance <= hearingRadius)
                {
                    zombie.OnGunShotHeard(shotPosition);
                }
            }
        }
    }
}
