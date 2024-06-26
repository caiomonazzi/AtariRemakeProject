using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


namespace Berzerk
{
    public class Player : MonoBehaviour
    {
        #region Variables:
        public static Player Instance; // Singleton

        [Header("Components Settings")]
        public Rigidbody2D rb;
        public Slider healthSlider;
        public Text coins;
        private XPSystem xpSystem;

        [Header("Movement Settings")]
        public float moveSpeed = 5f;
        private Vector2 movement;
        private Vector2 lastDirection;

        [Header("Health")]
        private const float maxHealth = 100f;
        public float health;

        [Header("Weapons/Ammo")]
        private Shoot shot;
        public GameObject projectilePrefab;
        public float projectileSpeed = 10f;
        public float aimAssistAngleThreshold = 10f; // Angle threshold for aim assist
        private bool aimAssistActive = false;

        [Header("Keys")]
        private HashSet<int> keys;
        public Dictionary<int, bool> doorKeys = new Dictionary<int, bool>();

        [Header("Health Controller")]
        [HideInInspector] public bool isDead = false;
        [HideInInspector] public bool isHurting = false;
        public int coinCount; // Coin count
        [Header("Parameters")]
        public float timeToDamage; // Time for pause between AI damage
        private bool isDamaged;

        // Delegate to notify when the character dies.
        public delegate void CharacterDeath();
        public CharacterDeath OnDeath;

        #endregion

        private void Awake()
        {
            // Singleton
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);


            }
            else
            {
                Instance = this;
                keys = new HashSet<int>();
                DontDestroyOnLoad(gameObject);
            }

            xpSystem = FindFirstObjectByType<XPSystem>();
            if (xpSystem != null)
            {
                Debug.Log("XPSystem Loaded");
            }

            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            InitializePlayerComponents();
        }

        private void InitializePlayerComponents()
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                xpSystem = player.GetComponent<XPSystem>();

                Debug.Log("Player components initialized.");
            }
            else
            {
                Debug.LogWarning("Player not found in the scene.");
            }
        }

        private void Start()
        {
            // Initialize health to max health
            health = maxHealth;

            // Initialize coin count
            coinCount = 0;
            UpdateCoinCountText();

            shot = GetComponent<Shoot>();
        }


        private void Update()
        {
            // Update health bar slider value
            healthSlider.value = health;

            // Reload the scene if health drops below zero
            if (health <= 0)
            {
                SceneManager.LoadScene(this.name);
            }

            HandleMovementInput();
            HandleShootingInput();
        }

        private void FixedUpdate()
        {
            MovePlayer();
            RotatePlayer();
        }

        // Increase coin count and update UI
        public void IncreaseCoinCount()
        {
            coinCount++;
            UpdateCoinCountText();
        }

        // Update the coin count text
        private void UpdateCoinCountText()
        {
            if (coins != null)
            {
                coins.text = "Coins: " + coinCount;
            }
        }

        // Decrease the player's health
        public void DecreaseHealth()
        {
            health -= 10;
        }

        // Handle collisions with zombies
        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Zombie"))
            {
                DecreaseHealth();
            }
        }

        // Handle triggers with medkits and ammo boxes
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("Medkit"))
            {
                RestoreHealth();
                Destroy(collision.gameObject);
            }
            else if (collision.gameObject.CompareTag("AmmoBox"))
            {
                RefillAmmo();
                Destroy(collision.gameObject);
            }
        }

        // Restore health to max
        private void RestoreHealth()
        {
            health = maxHealth;
        }

        // Refill ammo to max
        private void RefillAmmo()
        {
            if (shot == null)
            {
                shot = GameObject.FindGameObjectWithTag("Player").GetComponent<Shoot>();
            }
            shot.currentAmmo = shot.maxAmmo;
        }


        private void HandleMovementInput()
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");

            movement = new Vector2(horizontal, vertical).normalized;

            if (movement != Vector2.zero)
            {
                if (!aimAssistActive || Vector2.Angle(lastDirection, movement) > aimAssistAngleThreshold)
                {
                    lastDirection = movement;
                    aimAssistActive = false; // Disable aim assist when changing direction significantly
                }
            }
        }

        private void HandleShootingInput()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (shot != null && !shot.isInteracting)
                {
                    Vector2 shootDirection = GetShootDirection();
                    shot.ShootLogic(shootDirection);
                }
            }
        }

        private void MovePlayer()
        {
            rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
        }

        private void RotatePlayer()
        {
            if (movement != Vector2.zero)
            {
                float angle = Mathf.Atan2(lastDirection.y, lastDirection.x) * Mathf.Rad2Deg - 90f;
                rb.rotation = angle;
            }
        }

        private Vector2 GetShootDirection()
        {
            Vector2 shootDirection = lastDirection;
            Vector2 playerPosition = rb.position;
            Vector2 nearestEnemyPosition = GetNearestEnemyPosition();

            if (nearestEnemyPosition != Vector2.zero)
            {
                Vector2 toTarget = (nearestEnemyPosition - playerPosition).normalized;
                float angle = Vector2.Angle(lastDirection, toTarget);
                if (angle <= aimAssistAngleThreshold)
                {
                    shootDirection = toTarget;
                    aimAssistActive = true;
                }
            }
            else
            {
                aimAssistActive = false;
            }

            return shootDirection;
        }

        private Vector2 GetNearestEnemyPosition()
        {
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Zombie");
            float minDistance = Mathf.Infinity;
            Vector2 nearestEnemyPosition = Vector2.zero;

            foreach (GameObject enemy in enemies)
            {
                float distance = Vector2.Distance(rb.position, enemy.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearestEnemyPosition = enemy.transform.position;
                }
            }

            return nearestEnemyPosition;
        }

        public bool HasKey(int keyID)
        {
            return keys.Contains(keyID);
        }

        public void AddKey(int keyID)
        {
            keys.Add(keyID);
            Debug.Log("Key added: " + keyID);
            // Update UI or play sound if needed
        }

        public void UseKey(int keyID)
        {
            if (keys.Contains(keyID))
            {
                keys.Remove(keyID);
                Debug.Log("Key used: " + keyID);
                // Update UI or play sound if needed
            }
        }
    }
}



/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Berzerk
{
    public class TopDownMovement : MonoBehaviour
    {
        [Header("Movement Settings")]
        public float moveSpeed = 5f;
        public Rigidbody2D rb;

        [Header("Shooting Settings")]
        public GameObject projectilePrefab;
        public float projectileSpeed = 10f;

        private Vector2 movement;
        private Vector2 lastDirection;

        private void Update()
        {
            HandleMovementInput();
            HandleShootingInput();
        }

        private void FixedUpdate()
        {
            MovePlayer();
            RotatePlayer();
        }

        private void HandleMovementInput()
        {
            movement.x = Input.GetAxisRaw("Horizontal");
            movement.y = Input.GetAxisRaw("Vertical");

            if (movement != Vector2.zero)
            {
                lastDirection = movement;
            }
        }

        private void HandleShootingInput()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Shoot();
            }
        }

        private void MovePlayer()
        {
            rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
        }

        private void RotatePlayer()
        {
            if (movement != Vector2.zero)
            {
                float angle = Mathf.Atan2(lastDirection.y, lastDirection.x) * Mathf.Rad2Deg - 90f;
                rb.rotation = angle;
            }
        }

        private void Shoot()
        {
            GameObject projectile = Instantiate(projectilePrefab, rb.position, Quaternion.identity);
            Rigidbody2D projectileRb = projectile.GetComponent<Rigidbody2D>();
            projectileRb.velocity = lastDirection * projectileSpeed;
            Destroy(projectile, 2f); // Destroy the projectile after 2 seconds to avoid clutter
        }
    }
}
*/