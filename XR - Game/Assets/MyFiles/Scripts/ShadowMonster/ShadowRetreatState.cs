using UnityEngine;

public class ShadowRetreatState : State
{
    private ShadowMonsterAgent sAgent;

    // FIX: Changed 'Agent' to 'ShadowMonsterAgent' in the constructor
    public ShadowRetreatState(ShadowMonsterAgent agent) : base(null) 
    { 
        sAgent = agent; 
    }

    public override void Enter()
    {
        if (sAgent.rend != null) sAgent.rend.material.color = Color.gray; 
        
        // FIX: Replaced the missing 'StopMoving()' method with direct NavMeshAgent code
        if (sAgent.navAgent.isOnNavMesh) sAgent.navAgent.isStopped = true;

        Vector3 retreatDirection = -sAgent.transform.forward * 15f;
        sAgent.transform.position += retreatDirection;

        Debug.Log("FLASHLIGHT HIT: ShadowMonster teleported away to reset.");
    }

    public override void Update()
    {
        sAgent.ChangeState(new ShadowIdleState(sAgent));
    }
}
