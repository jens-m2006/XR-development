using UnityEngine;
using UnityEngine.UI;

public class RightHandHUD : MonoBehaviour
{
    public Slider healthSlider;

    private void Start()
    {

        Player.OnHealthChanged += UpdateHealthBar;

        Player player = Object.FindFirstObjectByType<Player>();
        if (player != null)
        {
            UpdateHealthBar(player.maxHealth, player.maxHealth);
        }
    }

    private void OnDestroy() 
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
