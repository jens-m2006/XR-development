using UnityEngine;
using System.Collections;

public class TimedDamageTest : MonoBehaviour
{
    [Header("Schade Instellingen")]
    public float schadeHoeveelheid = 20f;
    public float wachtTijd = 10f;

    void Start()
    {
        // Start de timer zodra dit object geladen wordt
        StartCoroutine(DeelSchadeUitNaTijd());
    }

    IEnumerator DeelSchadeUitNaTijd()
    {
        Debug.Log($"[TEST] Timer gestart! Wachten op {wachtTijd} seconden...");
        
        // Wacht exact 10 seconden
        yield return new WaitForSeconds(wachtTijd);

        // Zoek de speler direct in de scene op
        Player playerScript = Object.FindFirstObjectByType<Player>();

        if (playerScript != null)
        {
            Debug.Log($"[TEST] {wachtTijd} seconden voorbij! Schade van {schadeHoeveelheid} wordt nu uitgedeeld.");
            playerScript.TakeDamage(schadeHoeveelheid);
        }
        else
        {
            Debug.LogError("[TEST] FOUT: Kan geen 'Player' script vinden in de scene!");
        }
    }
}
