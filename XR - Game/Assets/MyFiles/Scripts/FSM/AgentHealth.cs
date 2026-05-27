using UnityEngine;
using System;

public class AgentHealth : MonoBehaviour
{
    public static event Action<float, float> OnHealthChanged;
    public AudioClip hammerHit;
    public float maxHealth = 100f;
    private float currentHealth;

    private void Start()
    {
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Hammer")) return;

        float damage = CalculateHammerDamage();
        TakeDamage(damage);
    }

    private void TakeDamage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        AudioManager.Instance.PlaySFX(hammerHit);

        OnHealthChanged?.Invoke(currentHealth, maxHealth); 

        Debug.Log($"[AGENT] Geraakt! Damage: {damage} | Health: {currentHealth}/{maxHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private float CalculateHammerDamage()
    {
        Agent agent = GetComponentInParent<Agent>();
        if (agent == null || agent.waypoints == null) return 25f;

        int total = agent.waypoints.Length;
        int disabled = 0;

        foreach (Transform wp in agent.waypoints)
        {
            if (wp == null) continue;
            WaypointReceiver receiver = wp.GetComponent<WaypointReceiver>();
            if (receiver != null && receiver.IsThisWaypointDisabled())
                disabled++;
        }

        float t = total > 0 ? (float)disabled / total : 1f;
        return Mathf.Lerp(5f, 25f, t);
    }

    private void Die()
    {
        GameManager.Instance.EndLevelWithWin(0);
        gameObject.SetActive(false);
    }
}