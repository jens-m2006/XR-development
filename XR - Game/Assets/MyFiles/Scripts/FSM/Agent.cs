using UnityEngine;
using UnityEngine.AI;

public class Agent : MonoBehaviour
{
    // fields ------------------
    public Transform player;
    public Transform[] waypoints;
    public float detectionRange = 10f;
    public float damageRange = 2f; // Range in which agent can damage player
    public float viewDotThreshold = 0.3f;
    public float waypointDistance = 2f;
    public float checkpointDistance = 3f;
    public float maxBattery = 100f;
    public float battery = 100f;
    public float chaseTimeToDrain = 10f;
    public float fleeBatteryPercent = 0.25f; // Fraction of maxBattery to trigger fleeing

    private Vector3 startPosition;
    private Quaternion startRotation;

    private State currentState;
    private NavMeshAgent navAgent;
    [HideInInspector] public Renderer rend; 
    
    private int currentWaypointIndex = 0;
    private float chaseTimer = 0f;

    public float defaultSpeed = 3.5f; // zelfde waarde als je NavMeshAgent speed in Inspector

// Voeg deze methode toe:
    public void SetSpeed(float speed)
    {
        GetComponent<UnityEngine.AI.NavMeshAgent>().speed = speed;
    }



    // start------------
    void Start()
    {
        startPosition = transform.position;
        startRotation = transform.rotation;

        rend = GetComponent<Renderer>(); // render component for changing  color
        navAgent = GetComponent<NavMeshAgent>(); // Component for the navmesh

        if (player == null)
        {
            Player found = Object.FindFirstObjectByType<Player>();
            if (found != null)
            {
                player = found.transform;
            }
            else
            {
                GameObject playerObj = GameObject.FindWithTag("Player");
                if (playerObj != null)
                {
                    player = playerObj.transform;
                }
            }
        }

        if (player == null)
        {
            Debug.LogWarning("Agent: player transform is not assigned and no Player object was found.");
        }

        ChangeState(new PatrolState(this));
        GameManager.OnLevelReset += ResetAgent;
    }

    void Update()
    {
        if (currentState != null)
        {
            currentState.Update();
        }
    }

    public void ChangeState(State newState)
    {
        if (currentState != null)
        {
            currentState.Exit();
        }

        currentState = newState;

        if (currentState != null)
        {
            currentState.Enter();
        }
    }

    private void OnDestroy()
    {
        GameManager.OnLevelReset -= ResetAgent;
    }

    private void ResetAgent()
    {
        battery = maxBattery;
        chaseTimer = 0f;
        currentWaypointIndex = 0;

        if (navAgent != null)
        {
            navAgent.enabled = true;
            navAgent.ResetPath();
            navAgent.isStopped = true;
        }

        transform.position = startPosition;
        transform.rotation = startRotation;

        if (currentState != null)
        {
            currentState.Exit();
            currentState = null;
        }

        ChangeState(new PatrolState(this));
    }

    // checks eerst hoe ver weg en daarna of binnen gezichtsveld en daarna of er geen muur tussen zit
    public float DistanceToPlayer()
    {
        if (player == null)
        {
            return float.MaxValue;
        }

        return Vector3.Distance(transform.position, player.position);
    }

    public bool CanSeePlayer()
    {
        if (player == null) return false;

        Vector3 dir = (player.position - transform.position).normalized;
        float dot = Vector3.Dot(transform.forward, dir);

        if (dot < viewDotThreshold)
        {
            return false;
        }

        RaycastHit hit;
        Vector3 origin = transform.position + Vector3.up * 0.5f;

        if (Physics.Raycast(origin, dir, out hit, detectionRange))
        {
            if (hit.transform == player || hit.transform.root == player.root)
            {
                return true;
            }

            if (hit.transform.CompareTag("Player"))
            {
                return true;
            }

            return false;
        }

        return false;
    }









    // movemnts =========================
    public void MoveTowards(Vector3 target)
    {
        if (navAgent == null)
            return;

        if (!navAgent.enabled)
            navAgent.enabled = true;

        navAgent.isStopped = false;
        navAgent.SetDestination(target);
    }

    public void StopMoving()
    {
        navAgent.isStopped = true;
    }

    public Transform GetCurrentWaypoint()
    {
        if (waypoints == null || waypoints.Length == 0) return null;

        // Loop door alle waypoints om een enabled te vinden, startend bij currentWaypointIndex
        for (int i = 0; i < waypoints.Length; i++)
        {
            int index = (currentWaypointIndex + i) % waypoints.Length;
            Transform wp = waypoints[index];

            if (wp == null) continue;

            WaypointReceiver receiver = wp.GetComponent<WaypointReceiver>();
            if (receiver != null && receiver.IsThisWaypointDisabled()) continue;

            // Gevonden: zet index naar dit waypoint en geef het terug
            currentWaypointIndex = index;
            return wp;
        }

        return null; // Alle waypoints zijn disabled
    }

    public void MoveToNextWaypoint()
    {
        Transform waypoint = GetCurrentWaypoint();
        if (waypoint == null) return;

        MoveTowards(waypoint.position);

        if (Vector3.Distance(transform.position, waypoint.position) >= waypointDistance)
        {
            return;
        }

        // Stap naar het volgende waypoint en laat GetCurrentWaypoint disabled ones overslaan
        currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        ResetBattery();
    }

    public bool IsOnCheckpoint()
    {
        if (waypoints == null) return false;

        foreach (Transform waypoint in waypoints)
        {
            if (waypoint != null && Vector3.Distance(transform.position, waypoint.position) < checkpointDistance)
            {
                return true;
            }
        }

        return false;
    }

    public Transform GetBestFleeWaypoint()
    {
        if (waypoints == null || waypoints.Length == 0) 
        {
            return null;
        }

        Vector3 awayFromPlayer = transform.position - player.position;
        awayFromPlayer.y = 0f;
        awayFromPlayer.Normalize();

        Transform bestWaypoint = null;
        float bestScore = float.MinValue;

        foreach (Transform waypoint in waypoints)
        {
            if (waypoint == null) 
            {
                continue;
            }

            Vector3 toWaypoint = waypoint.position - transform.position;
            toWaypoint.y = 0f;
            float distanceFromPlayer = Vector3.Distance(waypoint.position, player.position);
            float directionScore = Vector3.Dot(awayFromPlayer, toWaypoint.normalized);
            float score = distanceFromPlayer + directionScore * 10f;

            if (score > bestScore)
            {
                bestScore = score;
                bestWaypoint = waypoint;
            }
        }

        return bestWaypoint;
    }


    public void ResetBattery() 
    {
        battery = maxBattery;
    }

    public void DrainBattery() 
    {
        battery = 0f;
    }

    public bool IsBatteryEmpty() 
    {
        return battery <= 0f;
    }

    public void UpdateChaseTimer(float deltaTime)
    {
        chaseTimer += deltaTime;
        if (chaseTimer >= chaseTimeToDrain) 
        {
            DrainBattery();
        }
    }

    public void ResetChaseTimer()
    {
        chaseTimer = 0f;
    }

    public Vector3 GetFleeDestination()
    {
        Vector3 away = transform.position - player.position;
        away.y = 0f;
        if (away == Vector3.zero) away = transform.forward;
        away.Normalize();
        return transform.position + away * 10f;
    }

    public string GetCurrentStateName()
    {
        return currentState != null ? currentState.GetType().Name : "";
    }
}