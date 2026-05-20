using UnityEngine;

public class QuitGame : MonoBehaviour
{
    public void QuitTheGame()
    {
        
        Application.Quit();

        
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}
