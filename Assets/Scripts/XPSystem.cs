using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Berzerk
{
    public class XPSystem : MonoBehaviour
    {
        private float scoreStore; // use this for player prefs

        [Header("Set In Inspector")]
        public float XP;
        public float xpIncrease;
        public Text XPText;

        private void Start()
        {
            // Load the stored XP value
            XP = PlayerPrefs.GetFloat("scoreStore", scoreStore);
        }

        private void Update()
        {
            // Update the XP display
            UpdateXPDisplay();
        }

        // Increase the player's XP
        public void IncreaseXP()
        {
            XP += xpIncrease;
            PlayerPrefs.SetFloat("scoreStore", XP);
            PlayerPrefs.Save();
        }

        // Update the XP text display
        private void UpdateXPDisplay()
        {
            scoreStore = PlayerPrefs.GetFloat("scoreStore", scoreStore);
            XPText.text = scoreStore.ToString();
        }

        public void ResetXP()
        {
            PlayerPrefs.DeleteKey("scoreStore");
            XP = 0;
            scoreStore = 0;
            UpdateXPDisplay();
        }
    }
}
