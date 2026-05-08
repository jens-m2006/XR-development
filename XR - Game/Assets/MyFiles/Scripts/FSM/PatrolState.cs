using UnityEngine;

public class PatrolState : State
{
    public PatrolState(Agent agent) : base(agent) { }

    public override void Enter()
    {
        agent.rend.material.color = Color.blue;
    }

    public override void Update()
    {
        agent.MoveToNextWaypoint();

        if (agent.DistanceToPlayer() < agent.detectionRange && agent.CanSeePlayer())
        {
            agent.ChangeState(new ChaseState(agent));
        }
    }
}
