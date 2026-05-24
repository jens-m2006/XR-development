using UnityEngine;

public static class SaveData
{
    // Keys for PlayerPrefs to prevent typos
    private const string HighScoreKey = "HighScore";
    private const string LastScoreKey = "LastScore";
    private const string WinsKey = "TotalWins";
    private const string LossesKey = "TotalLosses";

    // Save game data
    public static void SaveGameData(int highScore, int lastScore, int wins, int losses)
    {
        PlayerPrefs.SetInt(HighScoreKey, highScore);
        PlayerPrefs.SetInt(LastScoreKey, lastScore);
        PlayerPrefs.SetInt(WinsKey, wins);
        PlayerPrefs.SetInt(LossesKey, losses);
        
        PlayerPrefs.Save(); // Actually writes the data to disk
        Debug.Log("Game data successfully saved!");
    }

    // Load game data
    public static void LoadGameData(out int highScore, out int lastScore, out int wins, out int losses)
    {
        // The second parameter is the default value (0) if no data exists yet
        highScore = PlayerPrefs.GetInt(HighScoreKey, 0);
        lastScore = PlayerPrefs.GetInt(LastScoreKey, 0);
        wins = PlayerPrefs.GetInt(WinsKey, 0);
        losses = PlayerPrefs.GetInt(LossesKey, 0);
        Debug.Log("Game data successfully loaded!");
    }
}
