using UnityEngine;
using TMPro; // Needed to control TextMeshPro objects

public class ScoreDisplay : MonoBehaviour
{
    [Header("Score Text")]

    public TextMeshProUGUI lastScoreAmountText;


    public TextMeshProUGUI timesWonAmountText;


    public TextMeshProUGUI timesLostAmountText;

  
    public TextMeshProUGUI highScoreAmountText;

    // Tstart method for putting all ad zero
    private void Start()
    {
        if (lastScoreAmountText != null) lastScoreAmountText.text = "0";
        if (timesWonAmountText != null) timesWonAmountText.text = "0";
        if (timesLostAmountText != null) timesLostAmountText.text = "0";
        if (highScoreAmountText != null) highScoreAmountText.text = "0";
    }
}
