using UnityEngine;

public static class SaveData
{
    private const string HighScoreKey = "HighScore";
    private const string LastScoreKey = "LastScore";
    private const string WinsKey = "TotalWins";
    private const string LossesKey = "TotalLosses";
    
    // Nieuwe keys voor de audio-instellingen
    private const string MusicKey = "Settings_Music";
    private const string SfxKey = "Settings_Sfx";

    // Uitgebreide opslagmethode met de audio bools
    public static void SaveGameData(int highScore, int lastScore, int wins, int losses, bool music, bool sfx)
    {
        PlayerPrefs.SetInt(HighScoreKey, highScore);
        PlayerPrefs.SetInt(LastScoreKey, lastScore);
        PlayerPrefs.SetInt(WinsKey, wins);
        PlayerPrefs.SetInt(LossesKey, losses);
        
        // Sla bools op als 1 (true) of 0 (false)
        PlayerPrefs.SetInt(MusicKey, music ? 1 : 0);
        PlayerPrefs.SetInt(SfxKey, sfx ? 1 : 0);
        
        PlayerPrefs.Save(); 
        Debug.Log("Game data and audio settings successfully saved!");
    }

    // Uitgebreide laadmethode (standaard staan de audio bools op 'true')
    public static void LoadGameData(out int highScore, out int lastScore, out int wins, out int losses, out bool music, out bool sfx)
    {
        highScore = PlayerPrefs.GetInt(HighScoreKey, 0);
        lastScore = PlayerPrefs.GetInt(LastScoreKey, 0);
        wins = PlayerPrefs.GetInt(WinsKey, 0);
        losses = PlayerPrefs.GetInt(LossesKey, 0);
        
        // Laad in en zet om naar bool. Standaardwaarde is 1 (aan) als er nog geen save is
        music = PlayerPrefs.GetInt(MusicKey, 1) == 1;
        sfx = PlayerPrefs.GetInt(SfxKey, 1) == 1;
        
        Debug.Log("Game data and audio settings successfully loaded!");
    }

    // Nieuwe methode voor je RESET-knop
    public static void ResetAllStats()
    {
        PlayerPrefs.DeleteAll(); // Wist ALLE opgeslagen PlayerPrefs data
        PlayerPrefs.Save();
        Debug.Log("All game stats and settings have been completely reset!");
    }
}
