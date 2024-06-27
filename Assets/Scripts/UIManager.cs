using UnityEngine;
using UnityEngine.UI;

namespace Berzerk
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }
        public Slider healthSlider;
        public Text coinsText;
        public Text xpText;
        public Text ammoDisplay;

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
    }
}
