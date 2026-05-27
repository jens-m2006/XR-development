using UnityEngine;
using TMPro;

public class ScoreDisplay : MonoBehaviour
{
    [Header("Score Text UI Elements")]
    public TextMeshProUGUI lastScoreAmountText;
    public TextMeshProUGUI timesWonAmountText;
    public TextMeshProUGUI timesLostAmountText;
    public TextMeshProUGUI highScoreAmountText;

    private void Start()
    {
        GameManager.OnMenuStarted += UpdateScoreBoardVisuals;
        GameManager.OnStatsUpdated += UpdateScoreBoardVisuals;
        UpdateScoreBoardVisuals();
    }

    private void UpdateScoreBoardVisuals()
    {
        if (GameManager.Instance == null) return;

        if (lastScoreAmountText != null) 
            lastScoreAmountText.text = GameManager.Instance.lastScore.ToString();

        if (timesWonAmountText != null) 
            timesWonAmountText.text = GameManager.Instance.totalWins.ToString();

        if (timesLostAmountText != null) 
            timesLostAmountText.text = GameManager.Instance.totalLosses.ToString();

        if (highScoreAmountText != null) 
            highScoreAmountText.text = GameManager.Instance.highScore.ToString();

        Debug.Log("[UI BOARD] Visuals synchronized successfully!");
    }

    private void OnDestroy()
    {
        GameManager.OnMenuStarted -= UpdateScoreBoardVisuals;
    }
}
