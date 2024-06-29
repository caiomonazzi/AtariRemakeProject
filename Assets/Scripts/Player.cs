using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Berzerk
{
    [System.Serializable]
    public class PlayerData
    {
        public float health;
        public int coins;
        public int ammo;
        public HashSet<int> keys = new HashSet<int>();
    }

    public class Player : MonoBehaviour
    {
        public static Player Instance { get; private set; }
        private PlayerData playerData;
        private Vector3 startPosition; // Store the start position
        private Zombie zombie;

        [Header("Components Settings")]
        public Rigidbody2D rb;
        private XPSystem xpSystem;

        [Header("Movement Settings")]
        public float moveSpeed = 5f;
        private Vector2 movement;
        private Vector2 lastDirection;

        [Header("Health")]
        private const float maxHealth = 100f;

        [Header("Weapons/Ammo")]
        private Shoot shot;
        public GameObject projectilePrefab;
        public float projectileSpeed = 10f;
        public float aimAssistAngleThreshold = 10f; // Angle threshold for aim assist

        [Header("Keys")]
        private HashSet<int> keys;
        public Dictionary<int, bool> doorKeys = new Dictionary<int, bool>();

        [Header("Health Controller")]
        [HideInInspector] public bool isDead = false;
        [HideInInspector] public bool isHurting = false;

        [Header("Parameters")]
        public int coinCount; // Coin count
        public float timeToDamage; // Time for pause between AI damage

        private void Awake()
        {
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

            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            InitializePlayerComponents();
            // ResetPlayerData();
            LoadPlayerData();
            RefreshStartingPoint();
        }

        private void InitializePlayerComponents()
        {
            xpSystem = GetComponent<XPSystem>();
            shot = GetComponent<Shoot>();
            if (shot != null)
            {
                // Fetch the BulletPool tagged as "_bulletPool"
                GameObject bulletPoolObject = GameObject.FindGameObjectWithTag("_bulletPool");
                if (bulletPoolObject != null)
                {
                    shot.bulletPool = bulletPoolObject.GetComponent<BulletPool>();
                }
            }
        }

        private void Start()
        {
            startPosition = transform.position;
            InitializePlayerComponents();
            LoadPlayerData();
            UpdateUIReferences();
        }

        private void UpdateUIReferences()
        {
            UIManager uiManager = UIManager.Instance;
            if (uiManager != null)
            {
                uiManager.healthSlider.value = playerData.health;
                uiManager.coinsText.text = "Coins: " + coinCount;
                if (shot != null)
                {
                    uiManager.ammoDisplay.text = shot.currentAmmo.ToString();
                }
            }
        }

        private void Update()
        {
            UIManager uiManager = UIManager.Instance;
            if (uiManager != null)
                uiManager.healthSlider.value = playerData.health;

            if (playerData.health <= 0)
            {
                ResetPlayer();
            }

            HandleMovementInput();
            HandleShootingInput();
        }

        private void FixedUpdate()
        {
            MovePlayer();
            RotatePlayer();
        }

        public void IncreaseCoinCount()
        {
            coinCount++;
            UpdateCoinCountText();
            SavePlayerData();
        }

        private void UpdateCoinCountText()
        {
            UIManager uiManager = UIManager.Instance;
            if (uiManager != null)
            {
                uiManager.coinsText.text = "Coins: " + coinCount;
            }
        }

        public void DecreaseHealth(float damage)
        {
            playerData.health -= damage;
            SavePlayerData();
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Zombie"))
            {
                zombie = collision.gameObject.GetComponent<Zombie>();
                if (zombie != null)
                {
                    DecreaseHealth(zombie.damage);
                }
            }
        }


        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("Medkit"))
            {
                RestoreHealth();
                Destroy(collision.gameObject);
                SavePlayerData();
            }
            else if (collision.gameObject.CompareTag("AmmoBox"))
            {
                RefillAmmo();
                Destroy(collision.gameObject);
                SavePlayerData();
            }
        }

        private void RestoreHealth()
        {
            playerData.health = maxHealth;
        }

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
                lastDirection = movement;
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
                    SaveAmmo();
                }
            }
        }

        private void MovePlayer()
        {
            rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
        }

        private void RotatePlayer()
        {
            if (lastDirection != Vector2.zero)
            {
                float angle = Mathf.Atan2(lastDirection.y, lastDirection.x) * Mathf.Rad2Deg - 90f;
                rb.rotation = angle;
            }
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
                    shootDirection = Vector2.Lerp(lastDirection, toTarget, angle / aimAssistAngleThreshold);
                }
            }

            return shootDirection;
        }

        public bool HasKey(int keyID)
        {
            return keys.Contains(keyID);
        }

        public void AddKey(int keyID)
        {
            keys.Add(keyID);
            SaveKeys();
            Debug.Log("Key added: " + keyID);
        }

        public void UseKey(int keyID)
        {
            if (keys.Contains(keyID))
            {
                keys.Remove(keyID);
                SaveKeys();
                Debug.Log("Key used: " + keyID);
            }
        }

        private void SavePlayerData()
        {
            PlayerPrefs.SetFloat("PlayerHealth", playerData.health);
            PlayerPrefs.SetInt("PlayerCoins", coinCount);
            SaveAmmo();
            PlayerPrefs.Save();
        }

        private void LoadPlayerData()
        {
            playerData = new PlayerData();
            playerData.health = PlayerPrefs.GetFloat("PlayerHealth", maxHealth);
            coinCount = PlayerPrefs.GetInt("PlayerCoins", 0);
            LoadKeys();
            LoadAmmo();
        }

        private void SaveKeys()
        {
            PlayerPrefs.SetString("PlayerKeys", string.Join(",", playerData.keys));
            PlayerPrefs.Save();
        }

        private void LoadKeys()
        {
            playerData.keys.Clear();
            string savedKeys = PlayerPrefs.GetString("PlayerKeys", "");
            if (!string.IsNullOrEmpty(savedKeys))
            {
                string[] keyArray = savedKeys.Split(',');
                foreach (string key in keyArray)
                {
                    if (int.TryParse(key, out int keyID))
                    {
                        playerData.keys.Add(keyID);
                    }
                }
            }
        }

        public void SaveAmmo()
        {
            PlayerPrefs.SetInt("PlayerAmmo", shot.currentAmmo);
            PlayerPrefs.Save();
        }

        private void LoadAmmo()
        {
            if (shot != null)
            {
                shot.currentAmmo = PlayerPrefs.GetInt("PlayerAmmo", shot.maxAmmo);
            }
        }

        private void RefreshStartingPoint()
        {
            startPosition = transform.position;
        }

        private void ResetPlayer()
        {
            ResetPlayerData();
            transform.position = startPosition;
            InitializePlayerComponents();
            UpdateUIReferences();
            RefreshStartingPoint();
        }

        public void ResetPlayerData()
        {
            PlayerPrefs.DeleteKey("PlayerHealth");
            PlayerPrefs.DeleteKey("PlayerCoins");
            PlayerPrefs.DeleteKey("PlayerKeys");
            PlayerPrefs.DeleteKey("PlayerAmmo");
            PlayerPrefs.DeleteKey("scoreStore");

            playerData.health = maxHealth;
            coinCount = 0;
            playerData.keys.Clear();
            UpdateCoinCountText();
            UIManager uiManager = UIManager.Instance;
            if (uiManager != null)
                uiManager.healthSlider.value = playerData.health;

            if (shot != null)
            {
                shot.currentAmmo = shot.maxAmmo;
                SaveAmmo();
            }

            if (xpSystem != null)
            {
                xpSystem.ResetXP();
            }
        }
    }
}
