using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class ShadowMonsterAgent : MonoBehaviour
{
    
    public static System.Action<Vector3> OnShadowMonsterAlertTriggered;

    [Header("Player Connection")]
    public Transform playerCamera; 

    [Header("Movement Speeds")]
    public float stalkSpeed = 2f;    
    public float alertSpeed = 5f;    

    [Header("Distance Settings")]
    public float alertRange = 6f;    
    public float stopDistance = 2f; 

    [Header("Visibility Threshold")]
    [Range(0f, 1f)] public float lookThreshold = 0.7f; 

    private Vector3 startPosition;
    private Quaternion startRotation;

    protected State currentState;
    [HideInInspector] public NavMeshAgent navAgent;
    [HideInInspector] public Renderer rend;

    void Start()
    {
        rend = GetComponent<Renderer>();
        navAgent = GetComponent<NavMeshAgent>();
        
        startPosition = transform.position;
        startRotation = transform.rotation;
        navAgent.stoppingDistance = stopDistance;

        GameManager.OnLevelReset += ResetShadow;

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

    private void OnDestroy()
    {
        GameManager.OnLevelReset -= ResetShadow;
    }

    private void ResetShadow()
    {
        if (currentState != null)
        {
            currentState.Exit();
            currentState = null;
        }

        transform.position = startPosition;
        transform.rotation = startRotation;

        if (navAgent != null)
        {
            navAgent.ResetPath();
            navAgent.isStopped = true;
            navAgent.stoppingDistance = stopDistance;
            navAgent.speed = stalkSpeed;
        }

        ChangeState(new ShadowIdleState(this));
    }

    public bool IsPlayerLookingAtMe()
    {
        if (playerCamera == null) return false;

  
        Vector3 monsterPosFlat = new Vector3(transform.position.x, 0f, transform.position.z);
        Vector3 playerPosFlat = new Vector3(playerCamera.position.x, 0f, playerCamera.position.z);
        Vector3 toMonster = (monsterPosFlat - playerPosFlat).normalized;


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
