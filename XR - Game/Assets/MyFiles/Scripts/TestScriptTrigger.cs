using UnityEngine;

public class LevelStartTrigger : MonoBehaviour
{
    [Header("Instellingen")]
    [SerializeField] private string targetTag = "Player"; // Verander dit naar de tag van je speler
    [SerializeField] private bool destroyAfterTrigger = true; // Verwijdert de trigger na activatie

    private void OnTriggerEnter(Collider other)
    {
        // Controleer of het object dat de trigger raakt de juiste tag heeft
        if (other.CompareTag(targetTag))
        {
            // Controleer of de GameManager bestaat
            if (GameManager.Instance != null)
            {
                // Start het level via de GameManager
                GameManager.Instance.StartLevel();

                // Optioneel: vernietig de trigger zodat je hem niet per ongeluk vaker activeert
                if (destroyAfterTrigger)
                {
                    Destroy(gameObject);
                }
            }
            else
            {
                Debug.LogWarning("LevelStartTrigger: GameManager.Instance is niet gevonden in de scene!");
            }
        }
    }
}
