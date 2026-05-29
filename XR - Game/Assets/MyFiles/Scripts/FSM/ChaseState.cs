using UnityEngine;

public class ChaseState : State
{
    public ChaseState(Agent agent) : base(agent) { }

    private float lostSightTimer = 0f;
    private float lostSightTimeout = 2f;

    private float damagePerSecond = 30f;
    private float damageTimer = 0f;

    public override void Enter()
    {
        agent.rend.material.color = Color.red;
        agent.ResetChaseTimer();
    }

    public override void Update()
    {
        if (AllWaypointsDisabled())
        {
            agent.ChangeState(new FleeState(agent));
            return;
        }

        if (agent.CanSeePlayer())
        {
            lostSightTimer = 0f;
            agent.MoveTowards(agent.player.position);
            agent.UpdateChaseTimer(Time.deltaTime);

            // Only deal damage if player is within damage range
            if (agent.DistanceToPlayer() <= agent.damageRange)
            {
                damageTimer += Time.deltaTime;
                if (damageTimer >= 1f)
                {
                    damageTimer = 0f;
                    if (Player.Instance != null)
                    {
                        Player.Instance.TakeDamage(damagePerSecond);
                    }
                }
            }
            else
            {
                damageTimer = 0f; // Reset timer if player gets out of range
            }
        }
        else
        {
            lostSightTimer += Time.deltaTime;
        }

        if (agent.battery <= agent.maxBattery * agent.fleeBatteryPercent || agent.IsBatteryEmpty())
        {
            agent.ChangeState(new FleeState(agent));
            return;
        }

        if (lostSightTimer >= lostSightTimeout || agent.DistanceToPlayer() > agent.detectionRange * 1.5f)
        {
            agent.ChangeState(new PatrolState(agent));
            return;
        }
    }

    private bool AllWaypointsDisabled()
    {
        if (agent.waypoints == null) return true;
        foreach (Transform wp in agent.waypoints)
        {
            if (wp == null) continue;
            WaypointReceiver receiver = wp.GetComponent<WaypointReceiver>();
            if (receiver == null || !receiver.IsThisWaypointDisabled())
                return false;
        }
        return true;
    }
}