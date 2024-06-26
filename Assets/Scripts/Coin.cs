using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Berzerk
{
    public class Coin : Item
    {
        public void OnPickedUp() //Method Pick Up item
        {
            Debug.Log("Coin Added");

            // Increase the player's coin count
            Player.Instance.IncreaseCoinCount();
        }

        public override void OnTriggerEnter2D(Collider2D collision)
        {
            onPickedUp += OnPickedUp; //Add event to parent
            base.OnTriggerEnter2D(collision); //Parent method
        }
    }
}
