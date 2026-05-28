using UnityEngine;
using System;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    
    //EVENTS AND ACTIONS
    public static event Action OnMenuStarted;
    public static event Action OnLevelStarted;
    public static event Action OnLevelReset;
    public static event Action OnStatsUpdated;

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
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        LoadStats();
    }

    private void Start()
    {
        // Start een veilige VR-opstartvertraging
        StartCoroutine(SafeVRStartSequence());
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            // Laat statische events intact, zodat UI objects die bij scene load opnieuw worden gemaakt
            // opnieuw kunnen subscriben en de scorebord update krijgen.
            Instance = null;
        }
    }


private System.Collections.IEnumerator SafeVRStartSequence()
{
    // Wacht exact 0.2 seconden tot de VR-headset en de level-vloer 100% stabiel geladen zijn
    yield return new WaitForSeconds(0.2f);

    // FIX: Zet de status heel even 'nep' op LevelPlay, zodat UpdateState(GameState.Menu) 
    // gegarandeerd de switch maakt, EnterMenuState() opstart en OnMenuStarted vlijmscherp afvuurt!
    currentState = GameState.LevelPlay;
    
    // 3. Activeer nu pas de Menu status en update het UI-bord
    UpdateState(GameState.Menu);
    
    Debug.Log("[GAMEMANAGER] Safe VR startup complete: UI refreshed and player is safely on the floor.");
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
            ResetLevelObjects();
            UpdateState(GameState.LevelPlay);
        }
    }

    public void CancelLevel()
    {
        if (currentState == GameState.LevelPlay)
        {
            currentLevelScore = 0;
            ResetLevelObjects();
            UpdateState(GameState.Menu);

            Debug.Log("[GAMEMANAGER] Switched to Menu without reloading scene.");
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
            ResetLevelObjects();
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
            ResetLevelObjects();
            UpdateState(GameState.Menu);
        }
    }
 
    private void EnterMenuState()
    {
        
        RenderVisibilityController.Instance.EnableMenu();
        RenderVisibilityController.Instance.DisableLevel();

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
        RenderVisibilityController.Instance.EnableLevel();
        RenderVisibilityController.Instance.DisableMenu();


        OnLevelStarted?.Invoke();
        ApplyAudioState();
        
    }



    
    private void SaveStats()
    {
        SaveData.SaveGameData(highScore, lastScore, totalWins, totalLosses, isMusicEnabled, isSfxEnabled);
        Debug.Log($"[GAMEMANAGER] Saved stats: highScore={highScore}, lastScore={lastScore}, wins={totalWins}, losses={totalLosses}, music={isMusicEnabled}, sfx={isSfxEnabled}");
        OnStatsUpdated?.Invoke();
    }

    private void LoadStats()
    {
        // Haal alle data in één keer op uit je SaveData script
        SaveData.LoadGameData(out highScore, out lastScore, out totalWins, out totalLosses, out isMusicEnabled, out isSfxEnabled);
        ApplyAudioState();
        Debug.Log($"[GAMEMANAGER] Loaded stats: highScore={highScore}, lastScore={lastScore}, wins={totalWins}, losses={totalLosses}, music={isMusicEnabled}, sfx={isSfxEnabled}");
        OnStatsUpdated?.Invoke();
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

    private void ResetLevelObjects()
    {
        OnLevelReset?.Invoke();
        Debug.Log("[GAMEMANAGER] Level objects reset to their initial state.");
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
        
        // Forceer UI-update voor scorebord en settings toggles
        OnStatsUpdated?.Invoke();
    }

    private void OnApplicationQuit()
    {
        SaveStats();
    }

    private void OnDisable()
    {
        if (Instance == this)
        {
            SaveStats();
        }
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
