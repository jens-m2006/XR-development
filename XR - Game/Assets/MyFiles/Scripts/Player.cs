using UnityEngine;
using System;
using Oculus.Haptics;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    // FIX: De Singleton ingang die de monsters nodig hebben om dit script te vinden!
    public static Player Instance { get; private set; }

    //Action
    public static event Action<float, float> OnHealthChanged;

    //Variables
    public float maxHealth = 100f;
    private float currentHealth;

    public HapticSource schadeHapticSource; 

    private void Awake()
    {
        // FIX: Standaard Singleton-check bij het opstarten
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return; // Stop met de rest van de Awake als dit een dubbelganger is
        }

        currentHealth = maxHealth;
        GameManager.OnLevelReset += ResetHealth;
    }

    // FIX: Monsters sturen een int, dus we voegen een extra methode toe die de float-versie aanroept
    public void TakeDamage(int damage)
    {
        TakeDamage((float)damage);
    }

    public void TakeDamage(float damage)
    {
        if (damage <= 0) return; 

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        // OPLOSSING: We sturen de trilling direct naar BEIDE Meta Touch controllers!
        OVRInput.SetControllerVibration(0.8f, 1f, OVRInput.Controller.All);

        // Stop de trilling na 0.3 seconden zodat de controllers niet blijven trillen
        Invoke("StopVibration", 0.3f);

        Debug.Log("[HAPTICS] Controllers trillen correct via OVRInput.Controller.All!");

        // FIX: Als de gezondheid 0 bereikt, moet de speler ook echt doodgaan!
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void StopVibration()
    {
        OVRInput.SetControllerVibration(0f, 0f, OVRInput.Controller.All);
    }

    public void ReceiveHealth(float health)
    {
        currentHealth += health;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    private void Die()
    {
        GameManager.Instance.EndLevelWithLose(0);
    }

    private void ResetHealth()
    {
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

   


    public void RequestHealthUpdate()
    {
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    private void OnDestroy()
    {
        GameManager.OnLevelReset -= ResetHealth;
    }
}
