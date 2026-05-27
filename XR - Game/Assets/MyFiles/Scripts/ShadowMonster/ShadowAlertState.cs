using UnityEngine;

public class ShadowAlertState : State
{
    private ShadowMonsterAgent sAgent;
    private float alertTimer = 0f;
    private float timerInterval = 5f; 

    public ShadowAlertState(ShadowMonsterAgent agent) : base(null) 
    { 
        sAgent = agent; 
    }

    public override void Enter()
    {
        if (sAgent.rend != null) sAgent.rend.material.color = Color.orange; 
        if (sAgent.navAgent.isOnNavMesh)
        {
            sAgent.navAgent.isStopped = false;
            sAgent.navAgent.speed = sAgent.alertSpeed; 
        }

        TriggerAlarmAtPosition();
        alertTimer = 0f;

        if (sAgent.playerCamera != null)
        {
            ShadowMonsterAgent.OnShadowMonsterAlertTriggered?.Invoke(sAgent.playerCamera.position);
            Debug.Log("[SHADOW_RADIO] Broadcasted player location to the Robot Drone!");
        }
    }

    public override void Update()
    {
        float currentDistance = sAgent.DistanceToPlayer();

        // WATERPROOF BREAK: If the monster breaks the stop barrier
        if (currentDistance <= sAgent.stopDistance)
        {
            if (sAgent.navAgent.isOnNavMesh)
            {
                sAgent.navAgent.isStopped = true;
                sAgent.navAgent.velocity = Vector3.zero;
                sAgent.navAgent.ResetPath(); // CRUCIAL FIX: Deactivates Unity's internal path calculation completely
            }

            // Snap the agent strictly to the boundary line
            Vector3 directionToPlayer = (sAgent.playerCamera.position - sAgent.transform.position).normalized;
            sAgent.transform.position = sAgent.playerCamera.position - (directionToPlayer * sAgent.stopDistance);
        }
        else
        {
            // Only move if alert speed allows active movement
            if (sAgent.navAgent.isOnNavMesh && sAgent.alertSpeed > 0f)
            {
                sAgent.navAgent.isStopped = false;
                sAgent.navAgent.speed = sAgent.alertSpeed;
                sAgent.navAgent.SetDestination(sAgent.playerCamera.position);
            }
            else if (sAgent.navAgent.isOnNavMesh && sAgent.alertSpeed <= 0f)
            {
                // Force a hard stop and erase any queued path calculations if speed is 0
                sAgent.navAgent.isStopped = true;
                sAgent.navAgent.velocity = Vector3.zero;
                sAgent.navAgent.ResetPath();
            }
        }

        // Pulse the location alarm message every 5 seconds
        alertTimer += Time.deltaTime;
        if (alertTimer >= timerInterval)
        {
            TriggerAlarmAtPosition();
            alertTimer = 0f; 
        }

        if (sAgent.IsPlayerLookingAtMe())
        {
            sAgent.ChangeState(new ShadowIdleState(sAgent));
            return;
        }

        if (currentDistance > sAgent.alertRange)
        {
            sAgent.ChangeState(new ShadowStalkState(sAgent));
            return;
        }
    }

    private void TriggerAlarmAtPosition()
    {
        Debug.Log("ALARM: ShadowMonster emitted a sound at its own position: " + sAgent.transform.position);
    }

    public override void Exit()
    {
        alertTimer = 0f;
    }
}
