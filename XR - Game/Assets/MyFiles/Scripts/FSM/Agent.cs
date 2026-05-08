using UnityEngine;
using UnityEngine.AI;

public class Agent : MonoBehaviour
{
    // fields ------------------
    public Transform player;
    public Transform[] waypoints;
    public float detectionRange = 10f;
    public float viewDotThreshold = 0.3f;
    public float waypointDistance = 2f;
    public float checkpointDistance = 3f;
    public float maxBattery = 100f;
    public float battery = 100f;
    public float chaseTimeToDrain = 10f;

    private State currentState;
    private NavMeshAgent navAgent;
    [HideInInspector] public Renderer rend; // For hiding the field in the inspecor while being public
    
    private int currentWaypointIndex = 0;
    private float chaseTimer = 0f;



    // start------------
    void Start()
    {
        rend = GetComponent<Renderer>(); // render component for changing  color
        navAgent = GetComponent<NavMeshAgent>(); // Component for the navmesh
        ChangeState(new PatrolState(this));
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

    // checks eerst hoe ver weg en daarna of binnen gezichtsveld en daanra of er geen muur tussne zit
    public float DistanceToPlayer()
    {
        return Vector3.Distance(transform.position, player.position);
    }

    public bool CanSeePlayer()
    {
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
            return hit.transform == player;
        }

        return false;
    }









    // movemnts =========================
    public void MoveTowards(Vector3 target)
    {
        navAgent.isStopped = false;
        navAgent.SetDestination(target);
    }

    public void StopMoving()
    {
        navAgent.isStopped = true;
    }

    public Transform GetCurrentWaypoint()
    {
        return waypoints == null || waypoints.Length == 0 ? null : waypoints[currentWaypointIndex];
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
}