using UnityEngine;

public class ChaseState : State
{
    public ChaseState(Agent agent) : base(agent) { }

    private float lostSightTimer = 0f;
    private float lostSightTimeout = 2f; // seconds to search before giving up

    public override void Enter()
    {
        agent.rend.material.color = Color.red;
        agent.ResetChaseTimer();
    }

    public override void Update()
    {
        // Move towards player if visible, otherwise increment lostSightTimer
        if (agent.CanSeePlayer())
        {
            lostSightTimer = 0f;
            agent.MoveTowards(agent.player.position);
            agent.UpdateChaseTimer(Time.deltaTime);
        }
        else
        {
            lostSightTimer += Time.deltaTime;
        }

        // If battery drops below configured fraction, switch to flee earlier
        if (agent.battery <= agent.maxBattery * agent.fleeBatteryPercent || agent.IsBatteryEmpty())
        {
            agent.ChangeState(new FleeState(agent));
            return;
        }

        // Only give up chase after the player has been out of sight for a short timeout
        if (lostSightTimer >= lostSightTimeout || agent.DistanceToPlayer() > agent.detectionRange * 1.5f)
        {
            agent.ChangeState(new PatrolState(agent));
            return;
        }
    }
}