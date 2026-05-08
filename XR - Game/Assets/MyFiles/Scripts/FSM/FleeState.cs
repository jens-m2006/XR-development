using UnityEngine;

public class FleeState : State
{
    public FleeState(Agent agent) : base(agent) { }

    public override void Enter()
    {
        agent.rend.material.color = Color.cyan;
    }

    public override void Update()
    {
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
}
