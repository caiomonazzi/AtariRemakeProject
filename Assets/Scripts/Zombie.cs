using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Berzerk
{
    public class Zombie : MonoBehaviour
    {
        private Transform player;

        [Header("Assign in Inspector")]
        public float speed;
        public float lineOfSite;
        public Rigidbody2D rb;

        private void Start()
        {
            // Find the player GameObject by tag
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }

        private void Update()
        {
            // Calculate the distance from the player
            float distanceFromPlayer = Vector2.Distance(player.position, transform.position);

            // Move the zombie if within line of sight
            if (distanceFromPlayer < lineOfSite)
            {
                MoveZombie();
            }
        }

        // Move the zombie towards the player and rotate it to face the player
        private void MoveZombie()
        {
            // Calculate the direction to the player
            Vector2 direction = (player.position - transform.position).normalized;

            // Move the zombie towards the player's position using Rigidbody2D to handle collisions
            Vector2 targetPosition = rb.position + direction * speed * Time.deltaTime;
            rb.MovePosition(targetPosition);

            // Rotate the zombie to face the player
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            rb.rotation = angle;
        }
    }
}
