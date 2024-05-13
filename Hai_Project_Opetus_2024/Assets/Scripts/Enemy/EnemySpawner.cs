using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public Transform playerTransform; // Assign the player's transform in the inspector
    public EnemyData[] enemyTypes; // Array of different enemy types
    public float spawnRadius = 10f; // Distance from the player where enemies will spawn
    public float spawnInterval = 2f; // Time between each spawn
    public int scoreThresholdForUpgrade = 1000;  // Score needed for each upgrade
    private int nextUpgradeScore = 1000;
    private float nextSpawnTime;
    private int upgradeLevel = 0;

    private void Start()
    {
        //playerTransform = GameObject.FindGameObjectWithTag("Player").transform; // Find the player
    }

    private void Update()
    {
        

        if (!GameManager.Instance.IsGameplay() || Time.time < nextSpawnTime) 
            return;

        if (playerTransform == null)
        {
            GetPlayer();
            return;
        }

        CheckUpgrade();
            SpawnEnemy();
            nextSpawnTime = Time.time + spawnInterval;
        
    }

    private void GetPlayer()
    {
       playerTransform = GameManager.Instance.getPlayer.gameObject.transform;
    }

    private void CheckUpgrade()
    {
        if (UIManager.Instance.GetCurrentScore() >= nextUpgradeScore)
        {
            upgradeLevel++;
            nextUpgradeScore += scoreThresholdForUpgrade;
        }
    }

    void SpawnEnemy()
    {
        // Random position around the player
        Vector2 spawnPos = playerTransform.position;
        spawnPos += Random.insideUnitCircle.normalized * spawnRadius;

        // Get a random enemy from the pool
        GameObject enemy = EnemyPoolManager.Instance.GetEnemy();
        enemy.transform.position = spawnPos;

        // Assign random enemy data from the list
        Enemy enemyScript = enemy.GetComponent<Enemy>();
        if (enemyScript != null)
        {
            enemyScript.enemyData = enemyTypes[Random.Range(0, enemyTypes.Length)];
            enemyScript.SetStats(upgradeLevel);
        }
    }
}
