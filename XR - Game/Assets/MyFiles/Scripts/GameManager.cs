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

    [Header("Audio Settings")]
    public bool isMusicEnabled;
    public bool isSfxEnabled;

    // Temporary score for the currently active level
    private int currentLevelScore;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); }
        else { Instance = this; }
    }

    private void Start()
    {
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
    // --- Pas deze twee bestaande methodes onderaan je GameManager aan ---
    private void SaveStats()
    {
        // Geef nu ook de audio statussen mee aan je opslagscript
        SaveData.SaveGameData(highScore, lastScore, totalWins, totalLosses, isMusicEnabled, isSfxEnabled);
    }

    private void LoadStats()
    {
        // Haal alle data in één keer op uit je SaveData script
        SaveData.LoadGameData(out highScore, out lastScore, out totalWins, out totalLosses, out isMusicEnabled, out isSfxEnabled);
        
        // Voer hier eventueel direct de actie uit om muziek/sfx te dempen op basis van de geladen bools
    }




    public void QuitGame()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; 
        #else
        Application.Quit(); 
        #endif
    }

    public void ResetGameStats()
    {
        SaveData.ResetAllStats();
        
        // Direct na het wissen laden we de schone fabrieksinstellingen in je GameManager
        LoadStats(); 
        
        // Optioneel: activeer events om je UI direct visueel te updaten naar 0/uit
        // bv. OnScoreChanged?.Invoke(0);
    }
}
