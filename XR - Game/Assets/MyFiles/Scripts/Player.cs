using UnityEngine;
using System;

public class Player : MonoBehaviour
{
    
    //Action
    public static event Action<float, float> OnHealthChanged;

    //Variables
    public float maxHealth = 100f;
    private float currentHealth;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
        
        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    public void ReceiveHealth(float health)
    {

    }

    private void Die()
    {
        GameManager.Instance.EndLevelWithLose(0);
    }

    
}
