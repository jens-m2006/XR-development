using UnityEngine;
using System;
using Oculus.Haptics;

public class Player : MonoBehaviour
{
    
    //Action
    public static event Action<float, float> OnHealthChanged;

    //Variables
    public float maxHealth = 100f;
    private float currentHealth;

    public HapticSource schadeHapticSource; 

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        if (damage <= 0) return; 

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        // OPLOSSING: We sturen de trilling direct naar BEIDE Meta Touch controllers!
        // OVRInput.Controller.All stuurt het signaal direct naar links én rechts tegelijk.
        OVRInput.SetControllerVibration(0.8f, 1f, OVRInput.Controller.All);

        // Stop de trilling na 0.3 seconden zodat de controllers niet blijven trillen
        Invoke("StopVibration", 0.3f);

        Debug.Log("[HAPTICS] Controllers trillen correct via OVRInput.Controller.All!");
    }

    private void StopVibration()
    {
        // Zet de trilling weer netjes op 0 voor beide handen
        OVRInput.SetControllerVibration(0f, 0f, OVRInput.Controller.All);
    }

    public void ReceiveHealth(float health)
    {

    }

    private void Die()
    {
        GameManager.Instance.EndLevelWithLose(0);
    }

    public void RequestHealthUpdate()
    {
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    
}
