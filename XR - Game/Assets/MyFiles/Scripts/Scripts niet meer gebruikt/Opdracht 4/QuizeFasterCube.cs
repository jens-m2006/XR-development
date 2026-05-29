using UnityEngine;

public class QuizeFasterCube : MonoBehaviour
{
    [Header("De Vallende Objecten")]
    public Rigidbody objectSneller; // Sleep hier de rode bal in
    public Rigidbody objectTrager;  // Sleep hier de blauwe bal in

    [Header("De Trigger Zones")]
    public GameObject triggerLinks;  
    public GameObject triggerRechts; 
    public GameObject triggerStart;  

    private int gekozenAntwoord = 0; 
    private Renderer kubusRenderer;

    void Start()
    {
        kubusRenderer = GetComponent<Renderer>();

        // We injecteren automatisch een klein hulpscript in de triggers.
        InstalleerTriggerHelper(triggerLinks, 1);
        InstalleerTriggerHelper(triggerRechts, 2);
        InstalleerTriggerHelper(triggerStart, 3);
    }

    private void InstalleerTriggerHelper(GameObject triggerObj, int id)
    {
        if (triggerObj != null)
        {
            TriggerHelper helper = triggerObj.AddComponent<TriggerHelper>();
            helper.zuil = this;
            helper.triggerId = id;
        }
        else
        {
            Debug.LogError("Oeps! Je bent vergeten een trigger-object te slepen in de Inspector van de Quiz-kubus!");
        }
    }

    // Deze functie wordt door de helpers aangeroepen als de speler erin stapt
    public void OntvangTriggerSignaal(int id)
    {
        if (id == 1)
        {
            gekozenAntwoord = 1;
            if (triggerLinks.GetComponent<Renderer>() != null)
                triggerLinks.GetComponent<Renderer>().material.color = Color.yellow;
            Debug.Log("Systeem: Linker optie geselecteerd!");
        }
        else if (id == 2)
        {
            gekozenAntwoord = 2;
            if (triggerRechts.GetComponent<Renderer>() != null)
                triggerRechts.GetComponent<Renderer>().material.color = Color.yellow;
            Debug.Log("Systeem: Rechter optie geselecteerd!");
        }
        else if (id == 3)
        {
            Debug.Log("Systeem: Start geactiveerd! Gekozen optie = " + gekozenAntwoord);
            if (gekozenAntwoord != 0)
            {
                StartSimulatie();
            }
            else
            {
                // Wordt blauw als je te vroeg op start drukt
                if (kubusRenderer != null) kubusRenderer.material.color = Color.blue; 
                Debug.LogWarning("Kies eerst links of rechts!");
            }
        }
    }

    void StartSimulatie()
    {
        // Physics activeren voor beide ballen zodat ze rollen
        if (objectSneller != null) { objectSneller.isKinematic = false; objectSneller.useGravity = true; }
        if (objectTrager != null) { objectTrager.isKinematic = false; objectTrager.useGravity = true; }

        // Beide antwoorden (1 en 2) zijn nu altijd juist!
        if (gekozenAntwoord == 1 || gekozenAntwoord == 2) 
        {
            if (kubusRenderer != null) kubusRenderer.material.color = Color.green;
            Debug.Log("BEIDE ZIJN JUIST! Massa heeft geen invloed op de valsnelheid zonder luchtweerstand.");
        }
    }
}

// DIT STUKJE CODE BLIJFT ONDERIN HETZELFDE BESTAND STAAN
public class TriggerHelper : MonoBehaviour
{
    public QuizeFasterCube zuil;
    public int triggerId;

    void OnTriggerEnter(Collider other)
    {
        // Zodra de speler (of de character controller) de trigger raakt
        if (other.CompareTag("Player") || other.GetComponent<CharacterController>() != null)
        {
            zuil.OntvangTriggerSignaal(triggerId);
        }
    }
}
