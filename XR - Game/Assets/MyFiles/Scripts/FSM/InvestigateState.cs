using UnityEngine;

public class InvestigateState : State
{
    private Vector3 investigateLocation;

    public InvestigateState(Agent agent, Vector3 location) : base(agent)
    {
        investigateLocation = location;
    }

    public override void Enter()
    {
        agent.rend.material.color = Color.yellow;
        agent.MoveTowards(investigateLocation);
    }

    public override void Update()
    {
        // Speler gezien -> chase
        if (agent.DistanceToPlayer() < agent.detectionRange && agent.CanSeePlayer())
        {
            agent.ChangeState(new ChaseState(agent));
            return;
        }

        // Locatie bereikt en niemand gezien -> terug naar patrol
        if (Vector3.Distance(agent.transform.position, investigateLocation) < agent.waypointDistance)
        {
            agent.ChangeState(new PatrolState(agent));
            return;
        }
    }
}