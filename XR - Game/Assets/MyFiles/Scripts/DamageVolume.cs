using UnityEngine;

public class DamageVolume : MonoBehaviour
{
    [Header("Schade Instellingen")]
    public float schadeHoeveelheid = 20f;

    private void OnTriggerEnter(Collider other)
    {
        // Check of het object dat de kubus aanraakt de Player is
        // (Zorg ervoor dat je PlayerRig de Tag "Player" heeft in Unity!)
        if (other.CompareTag("Player"))
        {
            Player playerScript = other.GetComponent<Player>();

            if (playerScript != null)
            {
                playerScript.TakeDamage(schadeHoeveelheid);
                Debug.Log("De kubus heeft " + schadeHoeveelheid + " schade uitgedeeld aan de speler!");
            }
        }
    }
}
