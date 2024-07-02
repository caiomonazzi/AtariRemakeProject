using System.Collections;
using UnityEngine;

namespace Berzerk
{
    public class Weapon : MonoBehaviour
    {
        private Player player; 

        [Header("Weapons/Ammo")]
        public GameObject projectilePrefab;
        public float aimAssistAngleThreshold = 11.25f; // Angle threshold for aim assist

        [Header("Ammo Settings")]
        public int maxAmmo = 300;
        public int currentAmmo;

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

        public void Initialize()
        {
            player = GetComponentInParent<Player>();
            muzzleFlash.SetActive(false);
            canShoot = true;

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
            else
            {
                canShoot = true;
            }
        }

        public void ShootLogic(Vector2 direction)
        {
            if (canShoot && !isInteracting)
            {
                currentAmmo--;

                gunShot?.Play();
                OnGunShot(transform.position, 50f); // Adjust the radius as needed

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
                canShoot = false;
                if (!flashing)
                {
                    StartCoroutine(Flashtime());
                }
            }
        }

        private Vector2 GetNearestEnemyPosition()
        {
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Zombie");
            float minDistance = Mathf.Infinity;
            Vector2 nearestEnemyPosition = Vector2.zero;

            foreach (GameObject enemy in enemies)
            {
                float distance = Vector2.Distance(player.rb.position, enemy.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearestEnemyPosition = enemy.transform.position;
                }
            }

            return nearestEnemyPosition;
        }

        public Vector2 GetShootDirection()
        {
            Vector2 shootDirection = player.lastDirection;
            Vector2 playerPosition = player.rb.position;
            Vector2 nearestEnemyPosition = GetNearestEnemyPosition();

            if (nearestEnemyPosition != Vector2.zero)
            {
                Vector2 toTarget = (nearestEnemyPosition - playerPosition).normalized;
                float angle = Vector2.Angle(player.lastDirection, toTarget);
                if (angle <= aimAssistAngleThreshold)
                {
                    shootDirection = Vector2.Lerp(player.lastDirection, toTarget, angle / aimAssistAngleThreshold);
                }
            }

            return shootDirection;
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
        }

        private IEnumerator ReturnBulletToPool(GameObject bullet, float delay)
        {
            yield return new WaitForSeconds(delay);
            bulletPool?.ReturnBullet(bullet);
        }

        private void OnGunShot(Vector2 shotPosition, float hearingRadius)
        {
            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(shotPosition, hearingRadius);
            foreach (var hitCollider in hitColliders)
            {
                if (hitCollider.CompareTag("Zombie"))
                {
                    Zombie nearbyZombie = hitCollider.GetComponent<Zombie>();
                    if (nearbyZombie != null)
                    {
                        nearbyZombie.OnGunShotHeard(shotPosition);
                    }
                }
            }
        }
    }
}