using UnityEngine;

public class ChaseState : State
{
    public ChaseState(Agent agent) : base(agent) { }

    public override void Enter()
    {
        agent.rend.material.color = Color.red;
        agent.ResetChaseTimer();
    }

    public override void Update()
    {
        agent.MoveTowards(agent.player.position);
        agent.UpdateChaseTimer(Time.deltaTime);

        if (agent.IsBatteryEmpty())
        {
            agent.ChangeState(new FleeState(agent));
            return;
        }

        if (agent.DistanceToPlayer() > agent.detectionRange)
        {
            agent.ChangeState(new PatrolState(agent));
        }
    }
}