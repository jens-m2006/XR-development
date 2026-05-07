using UnityEngine;

public class ScoreGetScene2 : MonoBehaviour
{
    
    void Start()
    {
        int newSceneScore = GameManager.Instance.score;
        Debug.Log($"the score in your new scene is {newSceneScore}");
    }

   
}
