using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    
    
    public static GameManager instance;

    //Score
    public int score = 10;


    if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        


  

    
}
