using UnityEngine;

public class ShadowStalkState : State
{
    private ShadowMonsterAgent sAgent;

    public ShadowStalkState(ShadowMonsterAgent agent) : base(null) 
    { 
        sAgent = agent; 
    }

    public override void Enter()
    {
        if (sAgent.rend != null) sAgent.rend.material.color = Color.red; 
        if (sAgent.navAgent.isOnNavMesh)
        {
            sAgent.navAgent.isStopped = false;
            sAgent.navAgent.speed = sAgent.stalkSpeed; 
        }
    }

    public override void Update()
    {
        // Fix: Properly reading sAgent.stopDistance
        if (sAgent.DistanceToPlayer() <= sAgent.stopDistance)
        {
            if (sAgent.navAgent.isOnNavMesh) sAgent.navAgent.isStopped = true;
        }
        else
        {
            if (sAgent.navAgent.isOnNavMesh)
            {
                sAgent.navAgent.isStopped = false;
                sAgent.navAgent.SetDestination(sAgent.playerCamera.position);
            }
        }

        if (sAgent.IsPlayerLookingAtMe())
        {
            sAgent.ChangeState(new ShadowIdleState(sAgent));
            return;
        }

        // Fix: Properly reading sAgent.stopDistance here as well
        if (sAgent.DistanceToPlayer() <= sAgent.alertRange && sAgent.DistanceToPlayer() > sAgent.stopDistance)
        {
            sAgent.ChangeState(new ShadowAlertState(sAgent));
        }
    }
}
