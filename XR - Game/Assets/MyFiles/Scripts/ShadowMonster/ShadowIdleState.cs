using UnityEngine;

public class ShadowIdleState : State
{
    private ShadowMonsterAgent sAgent;

    // FIX: Changed 'Agent' to 'ShadowMonsterAgent' in the constructor
    public ShadowIdleState(ShadowMonsterAgent agent) : base(null) 
    { 
        sAgent = agent; 
    }

    public override void Enter()
    {
        if (sAgent.rend != null) sAgent.rend.material.color = Color.yellow; 
        if (sAgent.navAgent.isOnNavMesh) sAgent.navAgent.isStopped = true;
    }

    public override void Update()
    {
        if (!sAgent.IsPlayerLookingAtMe())
        {
            sAgent.ChangeState(new ShadowStalkState(sAgent));
        }
    }
}
