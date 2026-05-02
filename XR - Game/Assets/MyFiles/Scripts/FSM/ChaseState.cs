using UnityEngine;

public class ChaseState : State
{
    public ChaseState(Agent agent) : base(agent) { }

    public override void Enter()
    {
        agent.rend.material.color = Color.red;
    }

    public override void Update()
    {
        agent.LookAtPlayer();
        agent.MoveTowards(agent.player.position);

        if (agent.DistanceToPlayer() > agent.detectionRange && !agent.IsInDarkZone())
        {
            agent.ChangeState(new IdleState(agent));
        }
    }
}