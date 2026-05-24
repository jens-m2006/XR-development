using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
    
    //EVENTS AND ACTIONS
    public static event Action OnMenuStarted;
    public static event Action OnLevelStarted; 
    
    
    public static GameManager Instance { get; private set; }

    public enum GameState { Menu, LevelPlay }
    [Header("Current State")]
    public GameState currentState;

    [Header("Game Statistics")]
    public int highScore;
    public int lastScore;
    public int totalWins;
    public int totalLosses;

    // Temporary score for the currently active level
    private int currentLevelScore;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); }
        else { Instance = this; }
    }

    private void Start()
    {
        // Load saved data as soon as the game starts
        LoadStats();
        UpdateState(GameState.Menu);
    }

    public void UpdateState(GameState newState)
    {
         if (currentState == newState) return;
        currentState = newState;
        switch (newState)
        {
            case GameState.Menu: 
                EnterMenuState(); 
                break;
            case GameState.LevelPlay: 
                EnterLevelPlayState(); 
                break;
        }
    }

    public void StartLevel()
    {
        if (currentState == GameState.Menu)
        {
            currentLevelScore = 0; // Reset score for the new level
            UpdateState(GameState.LevelPlay);
        }
    }

    public void CancelLevel()
    {
        if (currentState == GameState.LevelPlay)
        {
            UpdateState(GameState.Menu);
        }
    }

    // Win condition: Pass the score achieved in this level
    public void EndLevelWithWin(int scoreAchieved)
    {
        if (currentState == GameState.LevelPlay)
        {
            lastScore = scoreAchieved;
            totalWins++;

            // Check if it's a new high score
            if (lastScore > highScore)
            {
                highScore = lastScore;
                Debug.Log("New High Score!");
            }

            SaveStats(); // Save changes immediately
            UpdateState(GameState.Menu);
        }
    }

    // Loss condition
    public void EndLevelWithLose(int scoreAchieved)
    {
        if (currentState == GameState.LevelPlay)
        {
            lastScore = scoreAchieved;
            totalLosses++;

            SaveStats(); // Save changes immediately
            UpdateState(GameState.Menu);
        }
    }



    // --- State Logic Methods ---
    private void EnterMenuState()
    {
        OnMenuStarted?.Invoke();
    }
    private void EnterLevelPlayState()
    {
        OnLevelStarted?.Invoke();
    }



    // Data read and write
    private void SaveStats()
    {
        SaveData.SaveGameData(highScore, lastScore, totalWins, totalLosses);
    }
    private void LoadStats()
    {
        SaveData.LoadGameData(out highScore, out lastScore, out totalWins, out totalLosses);
    }




    public void QuitGame()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; 
        #else
        Application.Quit(); 
        #endif
    }
}
