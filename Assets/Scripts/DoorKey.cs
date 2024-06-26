using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Berzerk
{
    public class DoorKey : Item
    {
        [Header("Settings")]
        public int keyID;

        private void Start()
        {
            onPickedUp += OnPickedUp;
        }

        public void OnPickedUp()
        {
            Debug.Log("Key picked up: " + keyID);
            Player.Instance.AddKey(keyID);
        }

        public override void OnTriggerEnter2D(Collider2D collision)
        {
            base.OnTriggerEnter2D(collision);
        }
    }
}
