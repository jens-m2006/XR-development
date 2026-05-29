using System.Collections;
using UnityEngine;

public class SodaMachine : MonoBehaviour
{
    [Header("Dispense Instellingen")]
    public GameObject canPrefab;       // Sleep hier je blikje Prefab in via de Inspector
    public Transform spawnPoint;       // De plek waar het blikje naar buiten rolt (bijv. de bak onderaan)
    public float throwForce = 2f;       // Hoe hard het blikje eruit geduwd wordt

    public AudioClip vendingSound;

    [Header("Cooldown")]
    private bool isWaiting = false;     // Houdt bij of we in de 2 seconden wachtfase zitten

    // Dit activeert zodra de hamer de trigger van de automaat raakt
    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Hammer") && !isWaiting)
        {
            Debug.Log("Hammer entered trigger");
            DispenseCan();
        }
    }


    private void DispenseCan()
    {
        // Zet de automaat direct op 'wachten' zodat een tweede klap niks doet
        isWaiting = true;

        Debug.Log("KLOEK! Er komt een blikje aan...");
        AudioManager.Instance.PlaySFX(vendingSound);
        

        // Spawn het blikje op de positie van het spawnPoint
        GameObject newCan = Instantiate(canPrefab, spawnPoint.position, spawnPoint.rotation);

        // Geef het blikje een klein duwtje naar voren (als het blikje een Rigidbody heeft)
        Rigidbody canRb = newCan.GetComponent<Rigidbody>();
        if (canRb != null)
        {
            canRb.AddForce(spawnPoint.forward * throwForce, ForceMode.Impulse);
        }

        // Start een timer die na 2 seconden de automaat weer activeert
        StartCoroutine(ResetCooldownRoutine());
    }

    // Een Coroutine is een speciale Unity functie die kan pauzeren in de tijd
    private IEnumerator ResetCooldownRoutine()
    {
        // Wacht exact 2 seconden
        yield return new WaitForSeconds(2f);

        // De automaat is weer klaar voor de volgende klap!
        isWaiting = false;
        Debug.Log("Automaat is weer klaar voor gebruik.");
    }
}
