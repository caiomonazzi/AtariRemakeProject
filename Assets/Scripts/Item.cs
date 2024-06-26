using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Berzerk
{
    public enum ItemType { Key, Coin, Weapon, Gear, Potion, PowerUp }

    [RequireComponent(typeof(BoxCollider2D))]
    public class Item : MonoBehaviour
    {
        public ItemType type; // Item type

        public delegate void PickedUpAction(); // Delegate for pick up item
        public event PickedUpAction onPickedUp; // Pick up event

        public virtual void OnTriggerEnter2D(Collider2D collision) // If player entered in trigger
        {
            if (collision.gameObject.tag == "Player") // If it's player
            {
                onPickedUp?.Invoke(); // Event
                Destroy(gameObject); // Destroy this GameObject
            }
        }
    }
}
