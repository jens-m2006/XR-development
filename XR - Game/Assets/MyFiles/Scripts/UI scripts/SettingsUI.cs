using UnityEngine;
using UnityEngine.UI;

public class SettingsUI : MonoBehaviour
{
    [SerializeField] private Toggle musicToggle;
    [SerializeField] private Toggle sfxToggle;
    [SerializeField] private Button resetStatsButton;
    [SerializeField] private Button saveBackButton;


    private void Awake()
    {
        musicToggle.onValueChanged.AddListener(OnMusicToggled);
        sfxToggle.onValueChanged.AddListener(OnSfxToggled);
        resetStatsButton.onClick.AddListener(OnResetStatsClicked);
        saveBackButton.onClick.AddListener(OnSaveBackClicked);  

        musicToggle.isOn = GameManager.Instance.isMusicEnabled;
        sfxToggle.isOn = GameManager.Instance.isSfxEnabled;
    }

    private void OnDestroy()
    {
        musicToggle.onValueChanged.RemoveListener(OnMusicToggled);
        sfxToggle.onValueChanged.RemoveListener(OnSfxToggled);
        resetStatsButton.onClick.RemoveAllListeners();
        saveBackButton.onClick.RemoveAllListeners();
    }

    private void OnMusicToggled(bool isOn)
    {
        GameManager.Instance.SetMusicEnabled(isOn);
    }

    private void OnSfxToggled(bool isOn)
    {
        GameManager.Instance.SetSfxEnabled(isOn);
    }


    private void OnResetStatsClicked()
    {
        GameManager.Instance.ResetGameStats();
    }

    private void OnSaveBackClicked()
    {
        
    }
}
