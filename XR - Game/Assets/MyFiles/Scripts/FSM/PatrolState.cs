using UnityEngine;

public class PatrolState : State
{
    private Vector3 wanderTarget;
    private float wanderRadius = 5f;
    private float wanderTimer = 0f;
    private float wanderInterval = 3f; // elke X seconden nieuw punt

    public PatrolState(Agent agent) : base(agent) { }

    public override void Enter()
    {
        agent.rend.material.color = Color.blue;
        wanderTarget = Vector3.zero;
    }

    public override void Update()
    {
        if (AllWaypointsDisabled())
        {
            agent.ChangeState(new FleeState(agent));
            return;
        }
        
        
        if (CountActiveWaypoints() <= 1)
        {
            Wander();
        }
        else
        {
            agent.MoveToNextWaypoint();
        }

        if (agent.DistanceToPlayer() < agent.detectionRange && agent.CanSeePlayer())
        {
            agent.ChangeState(new ChaseState(agent));
        }
    }

    private void Wander()
    {
        // Ankerpunt: het laatste actieve waypoint, anders huidige positie
        Transform anchor = agent.GetCurrentWaypoint();
        Vector3 center = anchor != null ? anchor.position : agent.transform.position;

        wanderTimer += Time.deltaTime;

        // Eerste keer of timer verlopen: kies nieuw punt
        if (wanderTarget == Vector3.zero || wanderTimer >= wanderInterval)
        {
            wanderTimer = 0f;
            Vector2 rand = Random.insideUnitCircle * wanderRadius;
            wanderTarget = center + new Vector3(rand.x, 0f, rand.y);
        }

        agent.MoveTowards(wanderTarget);

        // Punt bereikt? Reset zodat er snel een nieuw gekozen wordt
        if (Vector3.Distance(agent.transform.position, wanderTarget) < agent.waypointDistance)
        {
            wanderTarget = Vector3.zero;
        }
    }

    private int CountActiveWaypoints()
    {
        if (agent.waypoints == null) return 0;

        int count = 0;
        foreach (Transform wp in agent.waypoints)
        {
            if (wp == null) continue;
            WaypointReceiver receiver = wp.GetComponent<WaypointReceiver>();
            if (receiver == null || !receiver.IsThisWaypointDisabled())
                count++;
        }
        return count;
    }

    private bool AllWaypointsDisabled()
    {
        if (agent.waypoints == null) return true;
        foreach (Transform wp in agent.waypoints)
        {
            if (wp == null) continue;
            WaypointReceiver receiver = wp.GetComponent<WaypointReceiver>();
            if (receiver == null || !receiver.IsThisWaypointDisabled())
                return false;
        }
        return true;
    }
}