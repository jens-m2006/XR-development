using UnityEngine;

public class FleeState : State
{
    public FleeState(Agent agent) : base(agent) { }

    private bool permanentFlee = false;

    public override void Enter()
    {
        agent.rend.material.color = Color.cyan;

        // Check of alle waypoints kapot zijn -> permanent flee
        permanentFlee = AllWaypointsDisabled();
    }

    public override void Update()
    {
        if (permanentFlee)
        {
            // Altijd actief van speler wegbewegen, geen state wissel
            agent.MoveTowards(agent.GetFleeDestination());
            return;
        }

        // Originele flee logica
        Transform checkpointTarget = agent.GetBestFleeWaypoint();
        if (checkpointTarget != null)
        {
            if (Vector3.Distance(agent.transform.position, checkpointTarget.position) < agent.checkpointDistance)
            {
                agent.ResetBattery();
                agent.ChangeState(new PatrolState(agent));
                return;
            }

            agent.MoveTowards(checkpointTarget.position);
            return;
        }

        agent.MoveTowards(agent.GetFleeDestination());
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