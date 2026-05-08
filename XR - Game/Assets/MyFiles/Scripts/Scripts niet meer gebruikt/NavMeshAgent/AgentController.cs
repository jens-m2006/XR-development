using UnityEngine;
using UnityEngine.AI; // Belangrijk: dit geeft toegang tot de NavMeshAgent

public class AgentController : MonoBehaviour
{
    [Header("Instellingen")]
    public Transform doelwit; // Sleep hier je Target (bijv. een Cube) naartoe in de Inspector

    private NavMeshAgent agent;

    void Start()
    {
        // Haal de NavMeshAgent component op die op deze Capsule staat
        agent = GetComponent<NavMeshAgent>();

        if (doelwit == null)
        {
            Debug.LogWarning("Vergeet niet een Doelwit (Target) te slepen in het script op: " + gameObject.name);
        }
    }

    void Update()
    {
        // Controleer of er een doelwit is ingesteld
        if (doelwit != null)
        {
            // Update de bestemming van de agent naar de huidige positie van het doelwit
            agent.destination = doelwit.position;
        }

        // Optioneel: Check of de agent dichtbij is (voor animaties of debugging)
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
            {
                // De agent is op zijn bestemming en staat stil
            }
        }
    }
}
