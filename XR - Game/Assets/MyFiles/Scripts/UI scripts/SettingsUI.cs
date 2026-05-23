using UnityEngine;
using UnityEngine.UI;

public class SettingsUI : MonoBehaviour
{
    [SerializeField] private Toggle musicToggle;
    [SerializeField] private Toggle sfxToggle;
    [SerializeField] private Button resetStatsButton;
    [SerializeField] private Button saveBackButton;

    [SerializeField] private AudioSource ambientMusicSource;

    private void Awake()
    {
        musicToggle.onValueChanged.AddListener(OnMusicToggled);
        sfxToggle.onValueChanged.AddListener(OnSfxToggled);
        resetStatsButton.onClick.AddListener(OnResetStatsClicked);
        saveBackButton.onClick.AddListener(OnSaveBackClicked);

        bool isMusicOn = PlayerPrefs.GetInt("IsMusicOn", 1) == 1;
        musicToggle.isOn = isMusicOn;
        UpdateMusicStatus(isMusicOn);
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
        PlayerPrefs.SetInt("IsMusicOn", isOn ? 1 : 0);
        PlayerPrefs.Save();
        UpdateMusicStatus(isOn);
    }

    private void UpdateMusicStatus(bool isOn)
    {
        if (ambientMusicSource != null)
        {
            ambientMusicSource.mute = !isOn;
        }
    }

    private void OnSfxToggled(bool isOn)
    {
        PlayerPrefs.SetInt("IsSfxOn", isOn ? 1 : 0);
        PlayerPrefs.Save();
    }

    private void OnResetStatsClicked()
    {
        PlayerPrefs.DeleteKey("PlayerStats"); 
        PlayerPrefs.Save();
    }

    private void OnSaveBackClicked()
    {
        // GameManager.Instance.LoadMainMenu();
    }
}
