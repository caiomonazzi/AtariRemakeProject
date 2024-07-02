using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "Readme", menuName = "Readme")]
public class Readme : ScriptableObject
{
    [TextArea(15, 20)]
    public string text;
}

[CustomEditor(typeof(Readme))]
public class ReadmeEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var readme = (Readme)target;
        GUIStyle titleStyle = new GUIStyle(GUI.skin.label) { fontSize = 18, fontStyle = FontStyle.Bold, wordWrap = true };
        GUIStyle headerStyle = new GUIStyle(GUI.skin.label) { fontSize = 14, fontStyle = FontStyle.Bold, wordWrap = true };
        GUIStyle textStyle = new GUIStyle(GUI.skin.label) { fontSize = 12, wordWrap = true };

        EditorGUILayout.LabelField("Game Design Document (GDD) for \"Maze Shooter - Berzerk Remake\"", titleStyle);
        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Table of Contents", headerStyle);
        EditorGUILayout.LabelField("Overview", textStyle);
        EditorGUILayout.LabelField("Game Concept", textStyle);
        EditorGUILayout.LabelField("Gameplay and Mechanics", textStyle);
        EditorGUILayout.LabelField("Movement", textStyle);
        EditorGUILayout.LabelField("Shooting", textStyle);
        EditorGUILayout.LabelField("Enemy AI", textStyle);
        EditorGUILayout.LabelField("Health and Damage", textStyle);
        EditorGUILayout.LabelField("Power-Ups and Collectibles", textStyle);
        EditorGUILayout.LabelField("Doors and Keys", textStyle);
        EditorGUILayout.LabelField("Next Level Door", textStyle);
        EditorGUILayout.LabelField("Level Design", textStyle);
        EditorGUILayout.LabelField("Visual and Audio Style", textStyle);
        EditorGUILayout.LabelField("Technical Specifications", textStyle);
        EditorGUILayout.LabelField("Development Plan", textStyle);
        EditorGUILayout.LabelField("Team Roles", textStyle);
        EditorGUILayout.LabelField("Appendices", textStyle);
        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Overview", headerStyle);
        EditorGUILayout.LabelField("Game Title: Maze Shooter - Berzerk Remake", textStyle);
        EditorGUILayout.LabelField("Genre: Action, Shooter, Arcade", textStyle);
        EditorGUILayout.LabelField("Platforms: PC", textStyle);
        EditorGUILayout.LabelField("Target Audience:", textStyle);
        EditorGUILayout.LabelField("Fans of classic arcade games,", textStyle);
        EditorGUILayout.LabelField("Players who enjoy fast-paced action shooters,", textStyle);
        EditorGUILayout.LabelField("Gamers seeking a blend of nostalgia and modern gameplay.", textStyle);
        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Game Concept", headerStyle);
        EditorGUILayout.LabelField("Immerse yourself in an updated maze shooter inspired by the classic Atari game Berzerk. Enjoy enhanced graphics and modern gameplay mechanics while navigating maze-like environments and fending off waves of enemies.", textStyle);
        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Gameplay and Mechanics", headerStyle);
        EditorGUILayout.LabelField("Movement", headerStyle);
        EditorGUILayout.LabelField("Controls:", textStyle);
        EditorGUILayout.LabelField("Joystick / Keyboard (W, A, S, D keys):", textStyle);
        EditorGUILayout.LabelField("Up: W / Joystick up", textStyle);
        EditorGUILayout.LabelField("Down: S / Joystick down", textStyle);
        EditorGUILayout.LabelField("Left: A / Joystick left", textStyle);
        EditorGUILayout.LabelField("Right: D / Joystick right", textStyle);
        EditorGUILayout.LabelField("Diagonals: Combination of W, A, S, D / Joystick diagonals", textStyle);
        EditorGUILayout.LabelField("Smooth transitions and animations for directional changes.", textStyle);
        EditorGUILayout.LabelField("Animator parameters dynamically reflect player states, including movement, shooting, and attacking.", textStyle);
        EditorGUILayout.LabelField("Features:", textStyle);
        EditorGUILayout.LabelField("Eight-direction movement: up, down, left, right, and diagonals.", textStyle);
        EditorGUILayout.LabelField("Fluid and responsive controls.", textStyle);
        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Shooting", headerStyle);
        EditorGUILayout.LabelField("Overview: The shooting system allows players to fire weapons in the direction the character is facing. Players switch between melee and ranged weapons, managing ammunition effectively with pickups scattered throughout the maze. The aimAssist feature helps correct the player's aim for more accurate shooting.", textStyle);
        EditorGUILayout.LabelField("Controls:", textStyle);
        EditorGUILayout.LabelField("Fire: Space bar / Action button", textStyle);
        EditorGUILayout.LabelField("Aim: Dictated by the character's facing direction", textStyle);
        EditorGUILayout.LabelField("Features:", textStyle);
        EditorGUILayout.LabelField("Directional Shooting: Fire weapons in the direction the character is facing.", textStyle);
        EditorGUILayout.LabelField("AimAssist: Subtle corrections to improve shooting accuracy.", textStyle);
        EditorGUILayout.LabelField("Ammunition Management: Players need to collect ammo pickups scattered throughout the maze.", textStyle);
        EditorGUILayout.LabelField("Switch Weapons: Players can switch between melee and ranged weapons based on their strategy and available resources.", textStyle);
        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Enemy AI", headerStyle);
        EditorGUILayout.LabelField("Behavior:", textStyle);
        EditorGUILayout.LabelField("Zombies patrol the maze, chase the player, and attack when within range.", textStyle);
        EditorGUILayout.LabelField("AI reacts dynamically to sounds such as gunshots, adjusting their state to chase or investigate.", textStyle);
        EditorGUILayout.LabelField("Features:", textStyle);
        EditorGUILayout.LabelField("Various zombie types with distinct behaviors.", textStyle);
        EditorGUILayout.LabelField("Enemies can be staggered, temporarily disabling their attacks and movement.", textStyle);
        EditorGUILayout.LabelField("AI states include patrolling, chasing, attacking, investigating, and fleeing.", textStyle);
        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Health and Damage", headerStyle);
        EditorGUILayout.LabelField("Health can be restored through pickups, and players must avoid enemy attacks and environmental hazards to survive.", textStyle);
        EditorGUILayout.LabelField("Player Health:", textStyle);
        EditorGUILayout.LabelField("Attributes:", textStyle);
        EditorGUILayout.LabelField("Max Health: The player has a maximum health of 100.", textStyle);
        EditorGUILayout.LabelField("Health UI: The player's current health is visually represented on a UI slider.", textStyle);
        EditorGUILayout.LabelField("Health Pickups: Medkits scattered throughout the maze can restore the player's health.", textStyle);
        EditorGUILayout.LabelField("Mechanics:", textStyle);
        EditorGUILayout.LabelField("Health Decrease: The player takes damage from enemy attacks and environmental hazards. The DecreaseHealth method reduces the player's health and triggers player death if health falls to zero.", textStyle);
        EditorGUILayout.LabelField("Health Restoration: Players can restore their health by collecting medkits, which call the RestoreHealth method to reset health to maximum.", textStyle);
        EditorGUILayout.LabelField("Damage:", textStyle);
        EditorGUILayout.LabelField("Player sustains damage from enemy attacks and environmental hazards.", textStyle);
        EditorGUILayout.LabelField("Constant tension to maintain health for survival.", textStyle);
        EditorGUILayout.LabelField("Attributes:", textStyle);
        EditorGUILayout.LabelField("Damage Amount: Each enemy type deals a specific amount of damage to the player on contact.", textStyle);
        EditorGUILayout.LabelField("Attack Interval: The frequency at which enemies can attack the player is managed by a cooldown period.", textStyle);
        EditorGUILayout.LabelField("Mechanics:", textStyle);
        EditorGUILayout.LabelField("Damage Application: When an enemy collides with or enters the player's trigger area, the DecreaseHealth method is called on the player, reducing their health by the enemy's damage amount.", textStyle);
        EditorGUILayout.LabelField("Attack Interval: A cooldown period prevents enemies from dealing continuous damage without a pause.", textStyle);
        EditorGUILayout.LabelField("Player Damage to Enemies:", textStyle);
        EditorGUILayout.LabelField("Attributes:", textStyle);
        EditorGUILayout.LabelField("Bullet Damage: Each bullet has a predefined damage value.", textStyle);
        EditorGUILayout.LabelField("Melee Damage: The melee weapon also has a predefined damage value.", textStyle);
        EditorGUILayout.LabelField("Stagger Effect: Hitting an enemy with a melee attack can stagger them, temporarily disabling their movement and attacks.", textStyle);
        EditorGUILayout.LabelField("Mechanics:", textStyle);
        EditorGUILayout.LabelField("Bullet Damage: When a bullet collides with an enemy, it calls the HandleZombieHit method, reducing the enemy's health by the bullet's damage amount.", textStyle);
        EditorGUILayout.LabelField("Melee Damage: The melee attack is initiated by calling the MeeleeAttack method, which enables the melee collider to detect hits and apply damage to enemies. Enemies hit by melee attacks may also be staggered.", textStyle);
        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Power-Ups and Collectibles", headerStyle);
        EditorGUILayout.LabelField("Types:", textStyle);
        EditorGUILayout.LabelField("Coins: Increase the player's score and unlock new features.", textStyle);
        EditorGUILayout.LabelField("Keys: Unlock doors to new areas and potential rewards.", textStyle);
        EditorGUILayout.LabelField("Ammo Boxes: Replenish ammunition to maxAmmo.", textStyle);
        EditorGUILayout.LabelField("Features:", textStyle);
        EditorGUILayout.LabelField("Enhance player abilities with various power-ups.", textStyle);
        EditorGUILayout.LabelField("Collectibles are strategically placed throughout the maze.", textStyle);
        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Doors and Keys", headerStyle);
        EditorGUILayout.LabelField("Mechanics:", textStyle);
        EditorGUILayout.LabelField("Keys: Players collect keys scattered throughout the maze. Each key has a unique ID and can unlock corresponding doors.", textStyle);
        EditorGUILayout.LabelField("Doors: Doors are placed at various points in the maze, blocking access to new areas. They can be unlocked using the correct key.", textStyle);
        EditorGUILayout.LabelField("Interaction: Players interact with doors by pressing the space bar. If the player has the correct key, the door unlocks and opens.", textStyle);
        EditorGUILayout.LabelField("Unlocking Doors: When a door is unlocked, the key is removed from the player's inventory, and the door becomes permanently open.", textStyle);
        EditorGUILayout.LabelField("Scripts:", textStyle);
        EditorGUILayout.LabelField("Door Script: Manages the locking and unlocking of doors. Checks for key presence and handles door opening animation and sound effects.", textStyle);
        EditorGUILayout.LabelField("DoorKey Script: Manages key pickups. Adds the key to the player's inventory and plays a pickup sound when collected.", textStyle);
        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Next Level Door", headerStyle);
        EditorGUILayout.LabelField("Mechanics:", textStyle);
        EditorGUILayout.LabelField("Locked Status: The door may start locked or unlocked. If locked, it requires specific conditions (e.g., collecting all keys or defeating all enemies) to unlock.", textStyle);
        EditorGUILayout.LabelField("Transition: When the player interacts with an unlocked Next Level Door, it triggers a scene transition to the next level.", textStyle);
        EditorGUILayout.LabelField("Persistence: Player data such as health, ammo, and inventory are carried over to the next level.", textStyle);
        EditorGUILayout.LabelField("Scripts:", textStyle);
        EditorGUILayout.LabelField("Next Level Door Script: Manages the transition to the next level. Ensures player data is saved and loaded correctly. Handles door interaction and scene transition animations.", textStyle);
        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Level Design", headerStyle);
        EditorGUILayout.LabelField("Design Elements:", textStyle);
        EditorGUILayout.LabelField("Various obstacles and enemy placements.", textStyle);
        EditorGUILayout.LabelField("Increasing difficulty with each level.", textStyle);
        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Visual and Audio Style", headerStyle);
        EditorGUILayout.LabelField("Visuals: Retro-inspired pixel art with modern enhancements for clarity and detail.", textStyle);
        EditorGUILayout.LabelField("Audio: 8 bit chiptunes", textStyle);
        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Development Tools", headerStyle);
        EditorGUILayout.LabelField("Game Engine: Unity", textStyle);
        EditorGUILayout.LabelField("Programming Language: C#", textStyle);
        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Team Roles", headerStyle);
        EditorGUILayout.LabelField("Game Designer:", textStyle);
        EditorGUILayout.LabelField("Programmer:", textStyle);
        EditorGUILayout.LabelField("Artist:", textStyle);
        EditorGUILayout.LabelField("Sound Designer:", textStyle);
        EditorGUILayout.LabelField("QA Tester:", textStyle);
        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Appendices", headerStyle);
        EditorGUILayout.LabelField("Miro Board", textStyle);
        EditorGUILayout.LabelField("Trello Board", textStyle);
        EditorGUILayout.LabelField("Discord", textStyle);
        EditorGUILayout.LabelField("Github link", textStyle);
        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Full Text View", headerStyle);
        readme.text = EditorGUILayout.TextArea(readme.text, textStyle);
    }
}
