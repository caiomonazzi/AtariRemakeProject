using System.Collections.Generic;
using UnityEngine;

namespace Berzerk
{
    public class BulletPool : MonoBehaviour
    {
        public int poolSize = 20;

        private Dictionary<GameObject, Queue<GameObject>> bulletPools;
        private Weapon weapon;

        private void Awake()
        {
            bulletPools = new Dictionary<GameObject, Queue<GameObject>>();
            weapon = FindObjectOfType<Weapon>();

            if (weapon != null)
            {
                InitializePoolForProjectile(weapon.projectilePrefab);
            }
            else
            {
                Debug.LogError("Weapon component not found in the scene.");
            }
        }

        private void InitializePoolForProjectile(GameObject projectilePrefab)
        {
            if (!bulletPools.ContainsKey(projectilePrefab))
            {
                Queue<GameObject> newPool = new Queue<GameObject>();
                for (int i = 0; i < poolSize; i++)
                {
                    GameObject bullet = Instantiate(projectilePrefab, transform);
                    bullet.GetComponent<Bullet>().Initialize(this);
                    bullet.SetActive(false);
                    newPool.Enqueue(bullet);
                }
                bulletPools.Add(projectilePrefab, newPool);
            }
        }

        public GameObject GetBullet()
        {
            if (weapon == null)
            {
                weapon = FindObjectOfType<Weapon>();
                if (weapon == null)
                {
                    Debug.LogError("Weapon component not found in the scene.");
                    return null;
                }
            }

            GameObject projectilePrefab = weapon.projectilePrefab;
            InitializePoolForProjectile(projectilePrefab);

            Queue<GameObject> pool = bulletPools[projectilePrefab];

            if (pool.Count > 0)
            {
                GameObject bullet = pool.Dequeue();
                if (bullet != null)
                {
                    bullet.SetActive(true);
                    return bullet;
                }
            }

            GameObject newBullet = Instantiate(projectilePrefab, transform);
            newBullet.GetComponent<Bullet>().Initialize(this);
            newBullet.SetActive(true);
            return newBullet;
        }

        public void ReturnBullet(GameObject bullet)
        {
            if (bullet != null)
            {
                bullet.SetActive(false);
                GameObject projectilePrefab = weapon.projectilePrefab;

                if (!bulletPools.ContainsKey(projectilePrefab))
                {
                    bulletPools[projectilePrefab] = new Queue<GameObject>();
                }

                bulletPools[projectilePrefab].Enqueue(bullet);
            }
        }
    }
}
