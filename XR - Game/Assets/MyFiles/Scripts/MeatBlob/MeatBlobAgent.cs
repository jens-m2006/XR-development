using UnityEngine;
using UnityEngine.AI;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]
public class MeatBlobAgent : MonoBehaviour
{
    [Header("Player Connection")]
    public Transform playerTransform; 

    [Header("Movement Settings")]
    public float roamSpeed = 1f;
    public float enragedSpeed = 4f;
    public float lungeRange = 3f;

    private float enrageDuration = 30f;
    private float enrageTimer = 0f;
    private Coroutine enrageCoroutine;

    private Vector3 startPosition;
    private Quaternion startRotation;
    private float startLungeRange;

    [HideInInspector] public NavMeshAgent navAgent;
    [HideInInspector] public bool isEnraged = false;
    
    private State currentState;
    [HideInInspector] public bool isLunging = false; // FIX: Prevents the attack from looping infinitely

    [Header("Sound Effects")]
    public AudioClip lungeSound; // Sleep hier je enge jumpscare schreeuw in via de Inspector


    void Start()
    {
        navAgent = GetComponent<NavMeshAgent>();
        navAgent.speed = roamSpeed;
        startLungeRange = lungeRange;
        startPosition = transform.position;
        startRotation = transform.rotation;

        BatteryController.OnAnyBatteryBroken += GoEnraged;
        GameManager.OnLevelReset += ResetBlob;

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

    

    private void OnDestroy()
    {
        BatteryController.OnAnyBatteryBroken -= GoEnraged;
        GameManager.OnLevelReset -= ResetBlob;
    }

    private void ResetBlob()
    {
        isEnraged = false;
        lungeRange = startLungeRange;
        isLunging = false;
        enrageTimer = 0f;

        if (enrageCoroutine != null)
        {
            StopCoroutine(enrageCoroutine);
            enrageCoroutine = null;
        }

        if (navAgent != null)
        {
            navAgent.enabled = true;
            navAgent.speed = roamSpeed;
            navAgent.ResetPath();
            navAgent.isStopped = false;
        }

        if (currentState != null)
        {
            currentState.Exit();
            currentState = null;
        }

        transform.position = startPosition;
        transform.rotation = startRotation;
        ChangeState(new BlobRoamState(this));
    }

    private void GoEnraged()
{
    enrageTimer += enrageDuration; // stapel seconden op bij meerdere invokes

    if (!isEnraged)
    {
        isEnraged = true;
        navAgent.speed = enragedSpeed;
        lungeRange *= 1.5f;
        Debug.Log("MEATBLOB: Enraged!");

        if (!isLunging)
            ChangeState(new BlobRoamState(this));
    }
    else
    {
        Debug.Log($"MEATBLOB: Extra invoke! Timer opgehoogd naar {enrageTimer}s");
    }

    // Stop vorige coroutine en start opnieuw met nieuwe timer
    if (enrageCoroutine != null)
        StopCoroutine(enrageCoroutine);

    enrageCoroutine = StartCoroutine(EnrageCountdown());
}

    private IEnumerator EnrageCountdown()
    {
        while (enrageTimer > 0f)
        {
            yield return null;
            enrageTimer -= Time.deltaTime;
        }

        // Terug naar normaal
        isEnraged = false;
        navAgent.speed = roamSpeed;
        lungeRange = startLungeRange;
        enrageCoroutine = null;

        Debug.Log("MEATBLOB: Enrage voorbij, terug naar normaal.");

        if (!isLunging)
            ChangeState(new BlobRoamState(this));
    }
}
