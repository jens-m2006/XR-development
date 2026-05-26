using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class ShadowMonsterAgent : MonoBehaviour
{
    [Header("Player Connection")]
    public Transform playerCamera; 

    [Header("Movement Speeds")]
    public float stalkSpeed = 2f;    
    public float alertSpeed = 5f;    

    [Header("Distance Settings")]
    public float alertRange = 6f;    
    public float stopDistance = 2f; // Check: Ensure this is spelled exactly like this 

    [Header("Visibility Threshold")]
    [Range(0f, 1f)] public float lookThreshold = 0.7f; 

    protected State currentState;
    [HideInInspector] public NavMeshAgent navAgent;
    [HideInInspector] public Renderer rend;

    void Start()
    {
        rend = GetComponent<Renderer>();
        navAgent = GetComponent<NavMeshAgent>();
        
        // Use the correctly spelled stopDistance here
        navAgent.stoppingDistance = stopDistance;

        ChangeState(new ShadowIdleState(this));
    }

    void Update()
    {
        if (currentState != null)
        {
            currentState.Update();
        }
    }

    public void ChangeState(State newState)
    {
        if (currentState != null) currentState.Exit();
        currentState = newState;
        if (currentState != null) currentState.Enter();
    }

    public bool IsPlayerLookingAtMe()
    {
        if (playerCamera == null) return false;

        // Calculate flat horizontal positions
        Vector3 monsterPosFlat = new Vector3(transform.position.x, 0f, transform.position.z);
        Vector3 playerPosFlat = new Vector3(playerCamera.position.x, 0f, playerCamera.position.z);
        Vector3 toMonster = (monsterPosFlat - playerPosFlat).normalized;

        // CRUCIAL VR FIX: Use playerCamera.transform.forward directly.
        // Ensure that in the Unity Inspector, your 'Player Camera' slot specifically 
        // contains the actual VR lens camera (like CenterEyeAnchor or Main Camera).
        Vector3 vrForward = playerCamera.forward;
        Vector3 playerForwardFlat = new Vector3(vrForward.x, 0f, vrForward.z).normalized;

        float dot = Vector3.Dot(playerForwardFlat, toMonster);
        return dot > lookThreshold;
    }


    public float DistanceToPlayer()
    {
        if (playerCamera == null) return float.MaxValue;
        return Vector3.Distance(transform.position, playerCamera.position);
    }

    public void TriggerFlashlightHit()
    {
        ChangeState(new ShadowRetreatState(this));
    }
}
