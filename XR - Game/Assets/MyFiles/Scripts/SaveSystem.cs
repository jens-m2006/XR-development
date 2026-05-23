using UnityEngine;

public static class SaveSystem
{
    public static void SaveData(GameData data)
    {
        PlayerPrefs.SetInt("LastScore", data.lastScore);
        PlayerPrefs.SetInt("TimesWon", data.timesWon);
        PlayerPrefs.SetInt("TimesLost", data.timesLost);
        PlayerPrefs.SetInt("HighScore", data.highScore);
        PlayerPrefs.SetInt("IsMusicOn", data.isMusicOn ? 1 : 0);
        PlayerPrefs.SetInt("IsSfxOn", data.isSfxOn ? 1 : 0);
        PlayerPrefs.Save();
    }

    public static GameData LoadData()
    {
        GameData data = new GameData();
        data.lastScore = PlayerPrefs.GetInt("LastScore", 0);
        data.timesWon = PlayerPrefs.GetInt("TimesWon", 0);
        data.timesLost = PlayerPrefs.GetInt("TimesLost", 0);
        data.highScore = PlayerPrefs.GetInt("HighScore", 0);
        data.isMusicOn = PlayerPrefs.GetInt("IsMusicOn", 1) == 1;
        data.isSfxOn = PlayerPrefs.GetInt("IsSfxOn", 1) == 1;
        return data;
    }

    public static void DeleteData()
    {
        PlayerPrefs.DeleteKey("LastScore");
        PlayerPrefs.DeleteKey("TimesWon");
        PlayerPrefs.DeleteKey("TimesLost");
        PlayerPrefs.DeleteKey("HighScore");
        PlayerPrefs.DeleteKey("IsMusicOn");
        PlayerPrefs.DeleteKey("IsSfxOn");
        PlayerPrefs.Save();
    }
}

[System.Serializable]
public class GameData
{
    public int lastScore = 0;
    public int timesWon = 0;
    public int timesLost = 0;
    public int highScore = 0;
    public bool isMusicOn = true;
    public bool isSfxOn = true;
}
