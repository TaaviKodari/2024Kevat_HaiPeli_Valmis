using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UIElements;

public class EnemyPoolManager : MonoBehaviour
{
    public static EnemyPoolManager Instance;

    public GameObject enemyPrefab; // The enemy prefab you want to pool
    public GameObject scoreDisplayPrefab;
    public GameObject dieEffect;
    public int poolSize = 10; // Initial size of the pool

    private Queue<GameObject> enemyPool = new Queue<GameObject>();

    private void Awake()
    {
        Instance = this;
        InitializePool();
    }

    private void InitializePool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject newEnemy = Instantiate(enemyPrefab);
            newEnemy.SetActive(false); // Start with the enemy deactivated
            enemyPool.Enqueue(newEnemy);
        }
    }

    public GameObject GetEnemy()
    {
        if (enemyPool.Count > 0)
        {
            GameObject enemy = enemyPool.Dequeue();
            enemy.SetActive(true);
            return enemy;
        }
        else
        {
            // Optionally expand the pool if empty
            GameObject newEnemy = Instantiate(enemyPrefab);
            newEnemy.SetActive(true);
            return newEnemy;
        }
    }

    public void ReturnEnemy(GameObject enemy)
    {
        enemy.SetActive(false);
        enemyPool.Enqueue(enemy);
    }
    public void EnemyDefeated(Vector3 position, int score)
    {
        // Instantiate the score display at the enemy's position
        GameObject effect = Instantiate(dieEffect, position, Quaternion.identity);
        GameObject scoreDisplay = Instantiate(scoreDisplayPrefab, position, Quaternion.identity);
        ScorePopUp displayScript = scoreDisplay.GetComponent<ScorePopUp>();
        if (displayScript != null)
        {
            int totalScore = score * UIManager.Instance.GetCurrentComboMultiplier();
            displayScript.SetScore(totalScore, true); // Set this to the score value from the enemy data
        }
    }

    public void SpawnDamageNumber(Vector3 position, int damage)
    {
        GameObject scoreDisplay = Instantiate(scoreDisplayPrefab, position, Quaternion.identity);
        ScorePopUp displayScript = scoreDisplay.GetComponent<ScorePopUp>();
        displayScript.SetScore(damage, false);
    }

    
}
