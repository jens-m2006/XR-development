using UnityEngine;

public class UIManager : MonoBehaviour
{
    [Header("Schermen")]
    public GameObject rightHandHUDCanvas; 

    private void OnEnable()
    {
        GameManager.OnMenuStarted += HideGameplayUI;
        GameManager.OnLevelStarted += ShowGameplayUI;
    }

    private void OnDisable()
    {
        GameManager.OnMenuStarted -= HideGameplayUI;
        GameManager.OnLevelStarted -= ShowGameplayUI;
    }

    private void Start()
    {
        // FORCEER HIER DIRECT VERBERGEN BIJ DE ALLEREERSTE START
        // Mocht het event te vroeg zijn afgevuurd, dan corrigeert dit het meteen.
        HideGameplayUI(); 
    }

    private void ShowGameplayUI()
    {
        if (rightHandHUDCanvas != null) rightHandHUDCanvas.SetActive(true); 
    }

    private void HideGameplayUI()
    {
        if (rightHandHUDCanvas != null) rightHandHUDCanvas.SetActive(false); 
    }
}
