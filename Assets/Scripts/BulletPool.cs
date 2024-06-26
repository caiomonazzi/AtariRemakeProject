using System.Collections.Generic;
using UnityEngine;

namespace Berzerk
{
    public class BulletPool : MonoBehaviour
    {
        public GameObject bulletPrefab;
        public int poolSize = 20;

        private Queue<GameObject> bulletPool;

        private void Awake()
        {
            bulletPool = new Queue<GameObject>();
            for (int i = 0; i < poolSize; i++)
            {
                GameObject bullet = Instantiate(bulletPrefab, transform);
                bullet.GetComponent<Bullet>().Initialize(this);
                bullet.SetActive(false);
                bulletPool.Enqueue(bullet);
            }
        }

        public GameObject GetBullet()
        {
            if (bulletPool.Count > 0)
            {
                GameObject bullet = bulletPool.Dequeue();
                if (bullet != null)
                {
                    bullet.SetActive(true);
                    return bullet;
                }
            }

            GameObject newBullet = Instantiate(bulletPrefab, transform);
            newBullet.GetComponent<Bullet>().Initialize(this);
            newBullet.SetActive(true);
            return newBullet;
        }

        public void ReturnBullet(GameObject bullet)
        {
            if (bullet != null)
            {
                bullet.SetActive(false);
                bulletPool.Enqueue(bullet);
            }
        }
    }
}
