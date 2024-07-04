using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum WeaponType { Melee, Ranged }

namespace Berzerk
{
    [System.Serializable]
    public class PlayerData
    {
        public float health;
        public int coins;
        public HashSet<int> keys = new HashSet<int>();
        public WeaponType currentWeaponType;
        public int currentAmmo;
    }

    public class Player : MonoBehaviour
    {
        public static Player Instance { get; private set; }
        private CameraFollow cameraFollow;
        public Animator characterAnimator;
        public SpriteRenderer characterSpriteRenderer;

        private PlayerData playerData;
        private Zombie zombie;

        [Header("Animator Settings")]
        public bool isMoving;
        public bool hasGun;
        public bool isAttacking;
        public bool isShooting;

        [Header("Components Settings")]
        public Rigidbody2D rb;
        private XPSystem xpSystem;
        private Weapon weapon;
        private MeleeWeapon meleeWeapon;

        [Header("Movement Settings")]
        public float moveSpeed = 5f;
        private Vector2 movement;
        public Vector2 lastDirection;

        [Header("Health")]
        private const float maxHealth = 100f;

        [Header("Keys")]
        private HashSet<int> keys;
        public Dictionary<int, bool> doorKeys = new Dictionary<int, bool>();

        [Header("Health Controller")]
        [HideInInspector] public bool isDead = false;
        [HideInInspector] public bool isHurting = false;

        [Header("Parameters")]
        public int coinCount; // Coin count
        public float timeToDamage; // Time for pause between AI damage

        [Header("Audio Settings")]
        public AudioSource sfxAudioSource; // Reference to the AudioSource component
        public AudioSource musicAudioSource; // Reference to the AudioSource component
        public AudioClip keyPickupSound; // The audio clip to play when a key is picked up
        public AudioClip healthPickupSound; // The audio clip to play when health is picked up
        public AudioClip ammoPickupSound; // The audio clip to play when ammo is picked up
        public AudioClip doorOpenSound; // The audio clip to play when a door is opened
        public AudioClip coinPickupSound; // The audio clip to play when a coin is picked up
        private AudioClip previousMusic;

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
            InitializeAnimatorParameters();

            // Ensure the camera follows the player after loading position
            if (cameraFollow != null)
            {
                cameraFollow.FindPlayer();
            }
        }

        private void InitializePlayerComponents()
        {
            xpSystem = GetComponent<XPSystem>();
            weapon = GetComponentInChildren<Weapon>();
            meleeWeapon = GetComponentInChildren<MeleeWeapon>();

            if (weapon != null)
            {
                GameObject bulletPoolObject = GameObject.FindGameObjectWithTag("_bulletPool");
                if (bulletPoolObject != null)
                {
                    weapon.bulletPool = bulletPoolObject.GetComponent<BulletPool>();
                }
            }
        }


        private void Start()
        {
            InitializePlayerComponents();
            LoadPlayerData();
            UpdateUIReferences();
            InitializeAnimatorParameters();

            // Find the CameraFollow script and assign the player
            cameraFollow = Camera.main.GetComponent<CameraFollow>();
            if (cameraFollow != null)
            {
                cameraFollow.FindPlayer();
            }
        }

        private void InitializeAnimatorParameters()
        {
            characterAnimator.SetBool("isMoving", isMoving);
            characterAnimator.SetBool("isShooting", isShooting);
            characterAnimator.SetBool("isAttacking", isAttacking);
            characterAnimator.SetBool("hasGun", hasGun);
        }

        private void UpdateUIReferences()
        {
            UIManager uiManager = UIManager.Instance;
            if (uiManager != null)
            {
                uiManager.healthSlider.value = playerData.health;
                uiManager.coinsText.text = "Coins: " + coinCount;
                if (weapon != null)
                {
                    uiManager.ammoDisplay.text = weapon.currentAmmo.ToString();
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
            HandleWeaponInput();
            SwitchWeaponIfNecessary(); // Ensure weapon switching logic is checked

            UpdateAnimatorParameters();
        }

        private void UpdateAnimatorParameters()
        {
            characterAnimator.SetBool("isMoving", isMoving);
            characterAnimator.SetBool("isShooting", isShooting);
            characterAnimator.SetBool("isAttacking", isAttacking);
            characterAnimator.SetBool("hasGun", hasGun);
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
                PickUpWeapon(collision.gameObject);
                RefillAmmo();
                SwitchToWeapon(WeaponType.Ranged);
                SavePlayerData();
                Destroy(collision.gameObject);
            }
        }

        private void RestoreHealth()
        {
            playerData.health = maxHealth;
        }

        private void RefillAmmo()
        {
            if (weapon != null)
            {
                weapon.currentAmmo = weapon.maxAmmo;
                SaveAmmo(); 
            }
        }

        private void HandleMovementInput()
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");

            movement = new Vector2(horizontal, vertical).normalized;

            if (movement != Vector2.zero)
            {
                lastDirection = movement;
                isMoving = true;
            }
            else
            {
                isMoving = false;
            }

            characterAnimator.SetBool("isMoving", isMoving);
        }


        private void HandleWeaponInput()
        {
            if (playerData.currentWeaponType == WeaponType.Ranged)
            {
                HandleShootingInput();
            }
            else if (playerData.currentWeaponType == WeaponType.Melee)
            {
                HandleMeleeInput();
            }
        }


        private void HandleShootingInput()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (weapon != null && !weapon.isInteracting && weapon.currentAmmo > 0)
                {
                    isShooting = true;
                    Vector2 shootDirection = weapon.GetShootDirection();
                    weapon.ShootLogic(shootDirection);
                    SaveAmmo();
                }
            }
            else if (Input.GetKeyUp(KeyCode.Space))
            {
                isShooting = false;
            }

            // Update animator
            characterAnimator.SetBool("isShooting", isShooting);
        }


        private void HandleMeleeInput()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (meleeWeapon != null)
                {
                    isAttacking = true;
                    meleeWeapon.MeeleeAttack();
                }
            }
            else if (Input.GetKeyUp(KeyCode.Space))
            {
                isAttacking = false;
            }

            // Update animator
            characterAnimator.SetBool("isAttacking", isAttacking);
        }

        private void SwitchToWeapon(WeaponType weaponType)
        {
            playerData.currentWeaponType = weaponType;
            hasGun = (weaponType == WeaponType.Ranged);

            // Update animator
            characterAnimator.SetBool("hasGun", hasGun);
        }

        public void PickUpWeapon(GameObject weaponObject)
        {
            Weapon newWeapon = weaponObject.GetComponent<Weapon>();
            if (newWeapon != null)
            {
                weapon = newWeapon;
                weapon.Initialize();
                playerData.currentWeaponType = WeaponType.Ranged;
                playerData.currentAmmo = weapon.currentAmmo;
                Destroy(weaponObject);
                InitializePlayerComponents();
                SavePlayerData();
                SwitchToWeapon(WeaponType.Ranged); // Ensure the animator is updated
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
            playerData.currentAmmo = weapon != null ? weapon.currentAmmo : 0;
            PlayerPrefs.SetInt("PlayerAmmo", playerData.currentAmmo);
            PlayerPrefs.SetInt("WeaponType", (int)playerData.currentWeaponType);

            SaveKeys();
            PlayerPrefs.Save();
        }


        private void LoadPlayerData()
        {
            playerData = new PlayerData();
            playerData.health = PlayerPrefs.GetFloat("PlayerHealth", maxHealth);
            coinCount = PlayerPrefs.GetInt("PlayerCoins", 0);
            playerData.currentAmmo = PlayerPrefs.GetInt("PlayerAmmo", 0); // Default to 0 if not found
            playerData.currentWeaponType = (WeaponType)PlayerPrefs.GetInt("WeaponType", (int)WeaponType.Melee);

            LoadKeys();

            // Ensure weapon is correctly initialized with the loaded data
            if (weapon != null)
            {
                weapon.currentAmmo = playerData.currentAmmo;
                if (playerData.currentWeaponType == WeaponType.Ranged)
                {
                    weapon.Initialize();
                }
            }

            SwitchWeaponIfNecessary();
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
            if (weapon != null)
            {
                PlayerPrefs.SetInt("PlayerAmmo", weapon.currentAmmo);
                PlayerPrefs.Save();
            }
        }


        private void LoadAmmo()
        {
            if (weapon != null)
            {
                weapon.currentAmmo = PlayerPrefs.GetInt("PlayerAmmo", weapon.maxAmmo);
            }
        }


        private void ResetPlayer()
        {
            ResetPlayerData();
            InitializePlayerComponents();
            UpdateUIReferences();
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

            if (weapon != null)
            {
                weapon.currentAmmo = weapon.maxAmmo;
                SaveAmmo();
            }

            if (xpSystem != null)
            {
                xpSystem.ResetXP();
            }
        }

        private void SwitchWeaponIfNecessary()
        {
            if (playerData.currentWeaponType == WeaponType.Ranged && (weapon == null || weapon.currentAmmo <= 0))
            {
                SwitchToWeapon(WeaponType.Melee);
            }
        }

        public void PlayCoinPickupSound()
        {
            PlaySound(coinPickupSound);
        }

        public void PlayKeyPickupSound()
        {
            PlaySound(keyPickupSound);
        }

        public void PlayHealthPickupSound()
        {
            PlaySound(healthPickupSound);
        }

        public void PlayAmmoPickupSound()
        {
            PlaySound(ammoPickupSound);
        }

        public void PlayDoorOpenSound()
        {
            PlaySound(doorOpenSound);
        }

        public void PlayMusic(AudioClip clip)
        {
            if (musicAudioSource != null && clip != null)
            {
                previousMusic = musicAudioSource.clip;
                musicAudioSource.clip = clip;
                musicAudioSource.Play();
            }
            else
            {
                Debug.LogWarning("Music AudioSource or AudioClip is missing.");
            }
        }

        public void StopMusic()
        {
            if (musicAudioSource != null)
            {
                musicAudioSource.Stop();
                if (previousMusic != null)
                {
                    musicAudioSource.clip = previousMusic;
                    musicAudioSource.Play();
                    previousMusic = null;
                }
            }
            else
            {
                Debug.LogWarning("Music AudioSource is missing.");
            }
        }

        private void PlaySound(AudioClip clip)
        {
            if (sfxAudioSource != null && clip != null)
            {
                sfxAudioSource.PlayOneShot(clip);
            }
            else
            {
                Debug.LogWarning("AudioSource or AudioClip is missing.");
            }
        }

        // Save the player's current position
        public void SavePlayerPosition()
        {
            PlayerPrefs.SetFloat("PlayerPosX", transform.position.x);
            PlayerPrefs.SetFloat("PlayerPosY", transform.position.y);
            PlayerPrefs.Save();
        }

        // Load the player's saved position
        private void LoadPlayerPosition()
        {
            float x = PlayerPrefs.GetFloat("PlayerPosX", transform.position.x);
            float y = PlayerPrefs.GetFloat("PlayerPosY", transform.position.y);
            transform.position = new Vector2(x, y);
        }


    }
}