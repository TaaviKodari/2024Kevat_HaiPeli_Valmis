using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable] // Make it visible in the Inspector
public struct EnemyUpgrade
{
    public int additionalHealth;
    public float additionalSpeed;
    public int additionalDamage;
    public Color color; // Color to represent this level of upgrade
}

[System.Serializable]
public struct DropItem
{
    public GameObject itemPrefab; // The prefab to drop
    public float dropChance; // Chance that this item will drop
}

[CreateAssetMenu(fileName = "NewEnemyData", menuName = "Enemy Data")]
public class EnemyData : ScriptableObject
{
    public string enemyName = "";
    [TextArea(3,10)]
    public string description = "";
    public float speed = 3f; // Movement speed of the enemy
    public int health = 10; // Health of the enemy
    public int damage = 1;
    public int scoreValue = 100;
    public List<DropItem> possibleDrops; // List of possible drops                                         // The prefab that this enemy can drop upon death
    public float dropChance = 0.1f; // Chance to drop the item (0.1 for 10% chance)
    public List<EnemyUpgrade> upgrades; // List of upgrades

    public GameObject GetRandomDrop()
    {
        float totalChance = 0f;
        foreach (var drop in possibleDrops)
        {
            totalChance += drop.dropChance;
        }

        float randomPoint = Random.value * totalChance;
        float currentPoint = 0f;

        foreach (var drop in possibleDrops)
        {
            currentPoint += drop.dropChance;
            if (currentPoint >= randomPoint)
            {
                return drop.itemPrefab;
            }
        }

        return null; // Return null if no drop is selected (e.g., if all chances are 0)
    }
}

