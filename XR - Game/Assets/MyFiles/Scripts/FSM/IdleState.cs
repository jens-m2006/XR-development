using UnityEngine;

public class IdleState : State
{
    public IdleState(Agent agent) : base(agent) { }

    public override void Enter()
    {
        agent.rend.material.color = Color.green;
    }

    public override void Update()
    {
        if (agent.DistanceToPlayer() < agent.detectionRange)
        {
            agent.ChangeState(new ChaseState(agent));
        }
    }
}