using UnityEngine;

public class TeleportState : State
{
    public TeleportState(Agent agent) : base(agent) { }

    public override void Enter()
    {
        agent.rend.material.color = Color.magenta;

        agent.TeleportBehindPlayer();

        agent.ChangeState(new ChaseState(agent));
    }
}