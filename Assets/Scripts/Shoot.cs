using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Berzerk
{
    public class Shoot : MonoBehaviour
    {
        [Header("General Settings")]
        public Transform firePoint;
        public float bulletForce;
        public AudioSource gunShot;
        private bool canShoot;
        public bool isInteracting;

        [Header("Fire Rate Settings")]
        public float fireRate = 5f;
        private float timeToFire;
        private float timeToFireMax = 1f;

        [Header("Muzzle Flash Settings")]
        public GameObject muzzleFlash;
        [Range(0, 5)]
        public int frameToFlash = 1;
        private bool flashing = false;

        [Header("Ammo Settings")]
        public int maxAmmo = 300;
        public int currentAmmo;

        [Header("Bullet Pool Settings")]
        public BulletPool bulletPool;

        private void Start()
        {
            Initialize();
        }

        private void Update()
        {
            HandleShootingCooldown();
            UpdateAmmoDisplay();
        }

        private void Initialize()
        {
            muzzleFlash.SetActive(false);
            canShoot = true;
            currentAmmo = maxAmmo;

            // Fetch the BulletPool tagged as "_bulletPool"
            GameObject bulletPoolObject = GameObject.FindGameObjectWithTag("_bulletPool");
            if (bulletPoolObject != null)
            {
                bulletPool = bulletPoolObject.GetComponent<BulletPool>();
            }
        }

        private void HandleShootingCooldown()
        {
            if (timeToFire > 0f)
            {
                timeToFire -= fireRate * Time.deltaTime;
            }
        }

        public void ShootLogic(Vector2 direction)
        {
            if (canShoot && timeToFire <= 0f && !isInteracting)
            {
                currentAmmo--;

                gunShot?.Play();

                GameObject bullet = bulletPool?.GetBullet();
                if (bullet != null)
                {
                    bullet.transform.position = firePoint.position;
                    bullet.transform.rotation = firePoint.rotation;
                    Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
                    rb.velocity = Vector2.zero;
                    rb.AddForce(direction * bulletForce, ForceMode2D.Impulse);
                    StartCoroutine(ReturnBulletToPool(bullet, 5f)); // Return bullet to pool after 5 seconds
                }

                timeToFire = timeToFireMax;
                if (!flashing)
                {
                    StartCoroutine(Flashtime());
                }
            }
        }

        private IEnumerator Flashtime()
        {
            muzzleFlash.SetActive(true);
            var frameflash = 0;
            flashing = true;
            while (frameflash <= frameToFlash)
            {
                frameflash++;
                yield return null;
            }
            muzzleFlash.SetActive(false);
            flashing = false;
        }

        private void UpdateAmmoDisplay()
        {
            UIManager uiManager = UIManager.Instance;
            if (uiManager != null && uiManager.ammoDisplay != null)
            {
                uiManager.ammoDisplay.text = currentAmmo.ToString();
            }
            canShoot = currentAmmo > 0;
        }

        private IEnumerator ReturnBulletToPool(GameObject bullet, float delay)
        {
            yield return new WaitForSeconds(delay);
            bulletPool?.ReturnBullet(bullet);
        }
    }
}
