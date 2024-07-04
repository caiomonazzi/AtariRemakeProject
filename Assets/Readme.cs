using UnityEngine;

[CreateAssetMenu(fileName = "Readme", menuName = "Readme")]
public class Readme : ScriptableObject
{
    [TextArea(15, 20)]
    public string text = @"Game Design Document (GDD) for ""Maze Shooter - Berzerk Remake""

Table of Contents
1. Overview
2. Game Concept
3. Gameplay and Mechanics
4. Movement
5. Shooting
6. Enemy AI
7. Health and Damage
8. Power-Ups and Collectibles
9. Doors and Keys
10. Next Level Door
11. Level Design
12. Visual and Audio Style
13. Technical Specifications
14. Development Plan
15. Team Roles
16. Appendices

1. Overview
Game Title: Maze Shooter - Berzerk Remake
Genre: Action, Shooter, Arcade
Platforms: PC
Target Audience:
- Fans of classic arcade games,
- Players who enjoy fast-paced action shooters,
- Gamers seeking a blend of nostalgia and modern gameplay.

2. Game Concept
Immerse yourself in an updated maze shooter inspired by the classic Atari game Berzerk. Enjoy enhanced graphics and modern gameplay mechanics while navigating maze-like environments and fending off waves of enemies.

3. Gameplay and Mechanics
Movement
Controls:
- Joystick / Keyboard (W, A, S, D keys):
- Up: W / Joystick up
- Down: S / Joystick down
- Left: A / Joystick left
- Right: D / Joystick right
- Diagonals: Combination of W, A, S, D / Joystick diagonals
Smooth transitions and animations for directional changes.
Animator parameters dynamically reflect player states, including movement, shooting, and attacking.
Features:
- Eight-direction movement: up, down, left, right, and diagonals.
- Fluid and responsive controls.

4. Shooting
Overview: The shooting system allows players to fire weapons in the direction the character is facing. Players switch between melee and ranged weapons, managing ammunition effectively with pickups scattered throughout the maze. The aimAssist feature helps correct the player's aim for more accurate shooting.
Controls:
- Fire: Space bar / Action button
- Aim: Dictated by the character's facing direction
Features:
- Directional Shooting: Fire weapons in the direction the character is facing.
- AimAssist: Subtle corrections to improve shooting accuracy.
- Ammunition Management: Players need to collect ammo pickups scattered throughout the maze.
- Switch Weapons: Players can switch between melee and ranged weapons based on their strategy and available resources.

5. Enemy AI
Behavior:
- Zombies patrol the maze, chase the player, and attack when within range.
- AI reacts dynamically to sounds such as gunshots, adjusting their state to chase or investigate.
Features:
- Various zombie types with distinct behaviors.
- Enemies can be staggered, temporarily disabling their attacks and movement.
- AI states include patrolling, chasing, attacking, investigating, and fleeing.

6. Health and Damage
Health can be restored through pickups, and players must avoid enemy attacks and environmental hazards to survive.
Player Health:
Attributes:
- Max Health: The player has a maximum health of 100.
- Health UI: The player's current health is visually represented on a UI slider.
- Health Pickups: Medkits scattered throughout the maze can restore the player's health.
Mechanics:
- Health Decrease: The player takes damage from enemy attacks and environmental hazards. The DecreaseHealth method reduces the player's health and triggers player death if health falls to zero.
- Health Restoration: Players can restore their health by collecting medkits, which call the RestoreHealth method to reset health to maximum.
Damage:
- Player sustains damage from enemy attacks and environmental hazards.
- Constant tension to maintain health for survival.
Attributes:
- Damage Amount: Each enemy type deals a specific amount of damage to the player on contact.
- Attack Interval: The frequency at which enemies can attack the player is managed by a cooldown period.
Mechanics:
- Damage Application: When an enemy collides with or enters the player's trigger area, the DecreaseHealth method is called on the player, reducing their health by the enemy's damage amount.
- Attack Interval: A cooldown period prevents enemies from dealing continuous damage without a pause.
Player Damage to Enemies:
Attributes:
- Bullet Damage: Each bullet has a predefined damage value.
- Melee Damage: The melee weapon also has a predefined damage value.
- Stagger Effect: Hitting an enemy with a melee attack can stagger them, temporarily disabling their movement and attacks.
Mechanics:
- Bullet Damage: When a bullet collides with an enemy, it calls the HandleZombieHit method, reducing the enemy's health by the bullet's damage amount.
- Melee Damage: The melee attack is initiated by calling the MeeleeAttack method, which enables the melee collider to detect hits and apply damage to enemies. Enemies hit by melee attacks may also be staggered.

7. Power-Ups and Collectibles
Types:
- Coins: Increase the player's score and unlock new features.
- Keys: Unlock doors to new areas and potential rewards.
- Ammo Boxes: Replenish ammunition to maxAmmo.
Features:
- Enhance player abilities with various power-ups.
- Collectibles are strategically placed throughout the maze.

8. Doors and Keys
Mechanics:
- Keys: Players collect keys scattered throughout the maze. Each key has a unique ID and can unlock corresponding doors.
- Doors: Doors are placed at various points in the maze, blocking access to new areas. They can be unlocked using the correct key.
- Interaction: Players interact with doors by pressing the space bar. If the player has the correct key, the door unlocks and opens.
- Unlocking Doors: When a door is unlocked, the key is removed from the player's inventory, and the door becomes permanently open.
Scripts:
- Door Script: Manages the locking and unlocking of doors. Checks for key presence and handles door opening animation and sound effects.
- DoorKey Script: Manages key pickups. Adds the key to the player's inventory and plays a pickup sound when collected.

9. Next Level Door
Mechanics:
- Locked Status: The door may start locked or unlocked. If locked, it requires specific conditions (e.g., collecting all keys or defeating all enemies) to unlock.
- Transition: When the player interacts with an unlocked Next Level Door, it triggers a scene transition to the next level.
- Persistence: Player data such as health, ammo, and inventory are carried over to the next level.
Scripts:
- Next Level Door Script: Manages the transition to the next level. Ensures player data is saved and loaded correctly. Handles door interaction and scene transition animations.

10. Level Design
Design Elements:
- Various obstacles and enemy placements.
- Increasing difficulty with each level.

11. Visual and Audio Style
Visuals: Retro-inspired pixel art with modern enhancements for clarity and detail.
Audio: 8-bit chiptunes

12. Technical Specifications
Game Engine: Unity
Programming Language: C#

13. Development Plan
14. Team Roles
- Game Designer:
- Programmer:
- Artist:
- Sound Designer:
- QA Tester:

15. Appendices
- Miro Board
- Trello Board
- Discord
- Github link
";
}
