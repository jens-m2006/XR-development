using UnityEngine;
using UnityEngine.UI;

public class RobotHUD : MonoBehaviour
{
    public Slider robotHealthSlider;
    public Slider robotBatterySlider;

    private void OnEnable()
    {
        AgentHealth.OnHealthChanged += UpdateHealthSlider;
    }

    private void OnDisable()
    {
        AgentHealth.OnHealthChanged -= UpdateHealthSlider;
    }

    private void UpdateHealthSlider(float current, float max)
    {
        if (robotHealthSlider == null) return;
        robotHealthSlider.value = current / max;
    }
}