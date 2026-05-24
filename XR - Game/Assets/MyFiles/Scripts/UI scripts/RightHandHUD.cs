using UnityEngine;
using UnityEngine.UI;

public class RightHandHUD : MonoBehaviour
{
    
    public Slider healthSlider;

    private void OnEnable()
    {
        Player.OnHealthChanged += UpdateHealthBar;
    }

    private void OnDisable()
    {
        Player.OnHealthChanged -= UpdateHealthBar;
    }

    private void UpdateHealthBar(float currentHealth, float maxHealth)
    {
        if (healthSlider != null)
        {
            healthSlider.value = currentHealth / maxHealth;
        }
    }
}
