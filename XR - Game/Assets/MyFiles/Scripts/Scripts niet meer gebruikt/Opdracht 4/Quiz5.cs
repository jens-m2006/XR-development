using UnityEngine;

public class Quiz5 : MonoBehaviour
{
    public enum WinnendeKant { Links, Rechts }

    [Header("Instelling Antwoord")]
    [Tooltip("Kies welke kant het juiste antwoord is voor deze specifieke opstelling")]
    public WinnendeKant correcteKeuze;

    [Header("De Fysieke Objecten")]
    public Rigidbody objectLinks; 
    public Rigidbody objectRechts;  

    [Header("De Trigger Zones")]
    public GameObject triggerLinks;  
    public GameObject triggerRechts; 
    public GameObject triggerStart;  

    [HideInInspector] public int gekozenAntwoord = 0; 
    private Renderer zuilRenderer;

    void Start()
    {
        zuilRenderer = GetComponent<Renderer>();

        if (objectLinks != null) objectLinks.isKinematic = true;
        if (objectRechts != null) objectRechts.isKinematic = true;

        KoppelTriggerAanZuil(triggerLinks, 1);
        KoppelTriggerAanZuil(triggerRechts, 2);
        KoppelTriggerAanZuil(triggerStart, 3);
    }

    private void KoppelTriggerAanZuil(GameObject triggerObj, int id)
    {
        if (triggerObj != null)
        {
            InternalTriggerHelper5 helper = triggerObj.AddComponent<InternalTriggerHelper5>();
            helper.hoofdZuil = this;
            helper.triggerId = id;
        }
        else
        {
            Debug.LogError("Oeps! Je bent vergeten een trigger-object te slepen in de Inspector van: " + gameObject.name);
        }
    }

    public void OntvangTriggerSignaal(int id, GameObject triggerObj)
    {
        if (id == 1)
        {
            gekozenAntwoord = 1;
            VeranderKleur(triggerObj, Color.yellow);
            Debug.Log(gameObject.name + " - Links gekozen!");
        }
        else if (id == 2)
        {
            gekozenAntwoord = 2;
            VeranderKleur(triggerObj, Color.yellow);
            Debug.Log(gameObject.name + " - Rechts gekozen!");
        }
        else if (id == 3)
        {
            if (gekozenAntwoord != 0)
            {
                StartSimulatie();
            }
            else
            {
                if (zuilRenderer != null) zuilRenderer.material.color = Color.blue; 
                Debug.LogWarning(gameObject.name + " - Kies eerst links of rechts!");
            }
        }
    }

    void StartSimulatie()
    {
        if (objectLinks != null) { objectLinks.isKinematic = false; objectLinks.useGravity = true; }
        if (objectRechts != null) { objectRechts.isKinematic = false; objectRechts.useGravity = true; }

        bool isCorrect = (correcteKeuze == WinnendeKant.Links && gekozenAntwoord == 1) || 
                         (correcteKeuze == WinnendeKant.Rechts && gekozenAntwoord == 2);

        if (zuilRenderer != null)
        {
            zuilRenderer.material.color = isCorrect ? Color.green : Color.red;
        }
    }

    private void VeranderKleur(GameObject obj, Color kleur)
    {
        if (obj != null && obj.GetComponent<Renderer>() != null)
            obj.GetComponent<Renderer>().material.color = kleur;
    }
}

public class InternalTriggerHelper5 : MonoBehaviour
{
    public Quiz5 hoofdZuil;
    public int triggerId;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.GetComponent<CharacterController>() != null)
        {
            if (hoofdZuil != null)
            {
                hoofdZuil.OntvangTriggerSignaal(triggerId, gameObject);
            }
        }
    }
}
