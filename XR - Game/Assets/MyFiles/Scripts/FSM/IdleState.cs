using UnityEngine;

public class IdleState : State
{
    public IdleState(Agent agent) : base(agent) { }

    public override void Enter()
    {
        agent.rend.material.color = Color.green;
        agent.StopMoving();
    }

    public override void Update()
    {
        // Alleen detecteren als speler binnen range én zichtbaar via dot + raycast
        if (agent.DistanceToPlayer() < agent.detectionRange && agent.CanSeePlayer())
        {
            agent.ChangeState(new ChaseState(agent));
        }
    }
}