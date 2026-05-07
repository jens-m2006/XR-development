using UnityEngine;
using UnityEngine.SceneManagement;

public class ScoreSetScene1 : MonoBehaviour
{
    
    
    void Start()
    {
        GameManager.Instance.score = 100;
        GameManager.Instance.AddScore(10);
    }

    
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
           SceneManager.LoadScene("Scene2"); 
        }
    
        
    
        
    }
        

    
}
