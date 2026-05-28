using UnityEngine;

public class InvestigateState : State
{
    private Vector3 investigateLocation;
    private float investigateSpeed = 6f; // aanpassen naar wens

    public InvestigateState(Agent agent, Vector3 location) : base(agent)
    {
        investigateLocation = location;
    }

    public override void Enter()
    {
        agent.rend.material.color = Color.yellow;
        agent.SetSpeed(investigateSpeed); // snelheid omhoog
        agent.MoveTowards(investigateLocation);
    }

    public override void Exit()
    {
        agent.SetSpeed(agent.defaultSpeed); // snelheid terug naar normaal
    }

    public override void Update()
    {
        if (agent.DistanceToPlayer() < agent.detectionRange && agent.CanSeePlayer())
        {
            agent.ChangeState(new ChaseState(agent));
            return;
        }

        if (Vector3.Distance(agent.transform.position, investigateLocation) < agent.waypointDistance)
        {
            agent.ChangeState(new PatrolState(agent));
            return;
        }
    }
}