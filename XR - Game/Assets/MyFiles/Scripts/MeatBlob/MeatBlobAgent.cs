using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class MeatBlobAgent : MonoBehaviour
{
    [Header("Player Connection")]
    public Transform playerTransform; 

    [Header("Movement Settings")]
    public float roamSpeed = 1f;
    public float enragedSpeed = 4f;
    public float lungeRange = 3f;

    [HideInInspector] public NavMeshAgent navAgent;
    [HideInInspector] public bool isEnraged = false;
    
    private State currentState;
    private bool isLunging = false; // FIX: Prevents the attack from looping infinitely

    [Header("Sound Effects")]
    public AudioClip lungeSound; // Sleep hier je enge jumpscare schreeuw in via de Inspector


    void Start()
    {
        navAgent = GetComponent<NavMeshAgent>();
        navAgent.speed = roamSpeed;

        BatteryController.OnAnyBatteryBroken += GoEnraged;

        ChangeState(new BlobRoamState(this));
    }

    void Update()
{
    if (currentState != null)
    {
        currentState.Update();
    }

    // Only check for a lunge if we haven't started lunging yet
    if (!isLunging && playerTransform != null)
    {
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
        
        // Check 1: Is the player within the attack range?
        if (distanceToPlayer <= lungeRange)
        {
            // Check 2: WATERPROOF WALL CHECK (Raycast)
            // Shoot a ray from the blob's center toward the player's center
            Vector3 directionToPlayer = (playerTransform.position - transform.position).normalized;
            RaycastHit hit;

            // Offset the starting point slightly upward so the ray doesn't hit the floor
            Vector3 origin = transform.position + Vector3.up * 0.5f;

            if (Physics.Raycast(origin, directionToPlayer, out hit, lungeRange))
            {
                // The blob ONLY lunges if the ray hits the player directly!
                // If it hits a wall or obstacle first, it will NOT trigger the lunge.
                if (hit.transform == playerTransform || hit.transform.gameObject.name.Contains("Player") || hit.transform.CompareTag("Player"))
                {
                    isLunging = true; 
                    ChangeState(new BlobLungeState(this));
                }
            }
        }
    }
}


    public void ChangeState(State newState)
    {
        if (currentState != null) currentState.Exit();
        currentState = newState;
        if (currentState != null) currentState.Enter();
    }

    private void GoEnraged()
    {
        if (isEnraged) return; 

        isEnraged = true;
        navAgent.speed = enragedSpeed;
        lungeRange *= 1.5f; // When enraged, the blob can lunge from further away!
        
        Debug.Log("MEATBLOB: A battery broke! The blob is now ENRAGED!");
        
        // Only force a state switch if we are not already dead/lunging
        if (!isLunging)
        {
            ChangeState(new BlobRoamState(this)); 
        }
    }

    private void OnDestroy()
    {
        BatteryController.OnAnyBatteryBroken -= GoEnraged;
    }
}
