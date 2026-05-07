using UnityEngine;

public class GameManager : Singleton<GameManager>
{

    public int score = 10;

    public void Start()
    {
        if(GameManager.Instance != null && GameManager.Instance != Instance)
        {
            DontDestroyOnLoad(this.gameObject);
        }
    }

    public void AddScore(int points)
    {
        
        Debug.Log($"there are{points} added to your score {score}");
        score += points;
    }

    
        
    

    
}


