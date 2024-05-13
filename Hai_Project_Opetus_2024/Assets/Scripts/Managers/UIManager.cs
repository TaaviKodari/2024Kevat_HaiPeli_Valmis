using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public TMP_Text scoreText; // Assign in the inspector
    public float comboDuration = 2f; // Time in seconds to get the next hit to continue the combo
    public Slider slider;
    public Slider playerHealthSlider;
    public TMP_Text playerHealthTxt;
    public Slider playerBulletLVLSlider;
    public TMP_Text playerBulletTxt;
    public GameObject gameOverScreen;
    private int totalScore = 0;

    private int comboMultiplier = 1;
    private float comboTimer;

    private void Awake()
    {
        // Ensure there's only one UIManager instance (Singleton pattern)
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        slider.maxValue = comboDuration;
    }

    public void AddScore(int score)
    {
        if (comboTimer <= 0)
        {
            comboMultiplier = 1;
        }

        totalScore += score * comboMultiplier;
        scoreText.text = "Score: " + totalScore + " x" + comboMultiplier; // Show the multiplier

        // Reset the combo timer
        
        comboTimer = comboDuration;
    }

    private void Update()
    {
        if (!GameManager.Instance.IsGameplay())
        {
            return;
        }
            // Decrease the combo timer
            if (comboTimer > 0)
        {
            comboTimer -= Time.deltaTime;
            slider.value = comboTimer;
        }
        else if (comboMultiplier > 1)
        {
            comboMultiplier = 1; // Reset multiplier if the timer has expired
            UpdateScoreDisplay(); // Update the score display to reflect the reset
        }
    }

    private void UpdateScoreDisplay()
    {
        scoreText.text = "Score: " + totalScore + " x" + comboMultiplier;
    }

    public void UpdatePlayerHealth(int currentHealth, int maxHealth)
    {
        playerHealthSlider.maxValue = maxHealth;
        playerHealthSlider.value = currentHealth;
        playerHealthTxt.text = $"{currentHealth} / {maxHealth}";
    }

    public void IncreaseComboMultiplier(int amount)
    {
        comboMultiplier += amount;
        comboTimer = comboDuration;
        UpdateScoreDisplay();
    }

    public void UpdateBulletLevel(int bulletLvl)
    {
        playerBulletLVLSlider.value = bulletLvl;
        playerBulletTxt.text = $"WPN:{bulletLvl}";
    }

    public int GetCurrentComboMultiplier()
    {
        return comboMultiplier;
    }

    public int GetCurrentScore()
    {
        return totalScore;
    }

    public void ActivateGameOverScreen()
    {
        gameOverScreen.SetActive(true);
        StartCoroutine(FadeToBlack());
    }

    private IEnumerator FadeToBlack()
    {
        float fadeDuration = 1f; // Duration of the fade
        float currentTime = 0f;
        Image goImage = gameOverScreen.GetComponent<Image>();

        while (currentTime < fadeDuration)
        {
            currentTime += Time.deltaTime;
            float alpha = Mathf.Lerp(0, 1, currentTime / fadeDuration);
            goImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }
    }

    public void RestartScene()
    {
        GameManager.Instance.RestartGame();
    }
}
