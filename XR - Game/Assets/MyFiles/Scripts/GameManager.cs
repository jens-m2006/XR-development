using UnityEngine;
using System;
using UnityEngine.SceneManagement;


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
            // 1. Update de status naar het menu
            UpdateState(GameState.Menu);

            // 2. Vraag de naam op van de huidige actieve scène
            string currentSceneName = SceneManager.GetActiveScene().name;

            // 3. Herlaad de scène direct om alles (monsters, lichten, batterijen) te resetten
            SceneManager.LoadScene(currentSceneName);
            
            Debug.Log("[GAMEMANAGER] Switched to Menu and reloaded the scene successfully.");
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
 
    private void EnterMenuState()
    {
        OnMenuStarted?.Invoke();
        // Ensure SFX stops immediately when entering menu
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.UpdateSfxPlayback(false);
        }
        ApplyAudioState();
    }

    private void EnterLevelPlayState()
    {
        OnLevelStarted?.Invoke();
        ApplyAudioState();
        
    }



    
    private void SaveStats()
    {
        SaveData.SaveGameData(highScore, lastScore, totalWins, totalLosses, isMusicEnabled, isSfxEnabled);
    }

    private void LoadStats()
    {
        // Haal alle data in één keer op uit je SaveData script
        SaveData.LoadGameData(out highScore, out lastScore, out totalWins, out totalLosses, out isMusicEnabled, out isSfxEnabled);
        ApplyAudioState();
    }




    private void ApplyAudioState()
    {
        if (AudioManager.Instance == null) return;

        // Ensure AudioManager knows the stored preference
        AudioManager.Instance.SetSFX(isSfxEnabled);

        // Music should only play in the Menu and when enabled in settings
        bool shouldPlayMusic = currentState == GameState.Menu && isMusicEnabled;
        AudioManager.Instance.UpdateMusicPlayback(shouldPlayMusic);

        // SFX are only allowed in LevelPlay and when enabled in settings
        bool shouldPlaySfx = currentState == GameState.LevelPlay && isSfxEnabled;
        AudioManager.Instance.UpdateSfxPlayback(shouldPlaySfx);
    }


    //UI settings buttons

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


    public void SetMusicEnabled(bool enabled)
    {
        isMusicEnabled = enabled;
        AudioManager.Instance.SetMusic(enabled);
        SaveStats();
        ApplyAudioState();
    }

    public void SetSfxEnabled(bool enabled)
    {
        isSfxEnabled = enabled;
        AudioManager.Instance.SetSFX(enabled);
        SaveStats();
    }


}
