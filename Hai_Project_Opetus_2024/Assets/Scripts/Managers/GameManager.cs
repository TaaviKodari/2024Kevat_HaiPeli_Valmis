using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameState currentState;
    public float cooldownTime = 2f;

    public PlayerController getPlayer { get; set; }
    private void Awake()
    {
        // Ensure there's only one GameManager instance (Singleton pattern)
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Optional: Keep the GameManager across scenes
        }
        else
        {
            Destroy(gameObject);
        }

        
    }

    private void Update()
    {
      
        switch (currentState)
        {
            case GameState.Gameplay:
                // Handle gameplay logic
                break;
            case GameState.Pause:
                // Handle pause logic
                break;
            case GameState.Cutscene:
                // Handle cutscene logic
                break;
            case GameState.Transition:
                // Handle transition logic
                break;
        }
    }

    public void StartGame(PlayerController player)
    {
        getPlayer = player;
        ChangeState(GameState.Gameplay);
    }


    // Method to change the game state
    public void ChangeState(GameState newState)
    {
        currentState = newState;
        OnStateChanged(newState);
    }

    // Handle any special logic when the state changes
    private void OnStateChanged(GameState newState)
    {
        switch (newState)
        {
            case GameState.Gameplay:
                // Resume gameplay activities
               // Time.timeScale = 1;
                break;
            case GameState.Pause:
                // Pause gameplay activities
              //  Time.timeScale = 0;
                break;
            case GameState.Cutscene:
                // Prepare and play cutscene
                break;
            case GameState.Transition:
                // Handle transitions between levels or scenes
                break;
        }
    }

    public bool IsGameplay()
    {
        return currentState == GameState.Gameplay;
    }
    public void PlayerDied()
    {
        StartCoroutine(DeathSequence());
    }

    private IEnumerator DeathSequence()
    {
        // Wait for the cooldown
        yield return new WaitForSeconds(cooldownTime);
        UIManager.Instance.ActivateGameOverScreen();
    }
    public void RestartGame()
    {
        // Reload the current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        ChangeState(GameState.Gameplay);
    }
}
