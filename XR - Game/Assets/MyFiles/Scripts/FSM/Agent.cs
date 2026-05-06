using UnityEngine;
using UnityEngine.AI;

public class Agent : MonoBehaviour
{
    public Transform player;
    public float speed = 3f;

    public float detectionRange = 10f;
    public float viewDotThreshold = 0.3f;

    private State currentState;
    private NavMeshAgent navAgent;

    [HideInInspector] public Renderer rend;

    private bool playerInDarkZone = false;
    private bool teleportScheduled = false;
    private float teleportTimer = 0f;

    void Start()
    {
        rend = GetComponent<Renderer>();
        navAgent = GetComponent<NavMeshAgent>();
        ChangeState(new IdleState(this));
    }

    void Update()
    {
        if (currentState != null)
            currentState.Update();

        HandleTeleportTimer();
    }

    public void ChangeState(State newState)
    {
        if (currentState != null)
            currentState.Exit();

        currentState = newState;
        currentState.Enter();
    }

    public float DistanceToPlayer()
    {
        return Vector3.Distance(transform.position, player.position);
    }

    public bool CanSeePlayer()
    {
        Vector3 dir = (player.position - transform.position).normalized;
        float dot = Vector3.Dot(transform.forward, dir);

        // Eerst dot product check — buiten kijkhoek? Stop meteen
        if (dot < viewDotThreshold) return false;

        // Dan raycast — zit er een muur tussen?
        RaycastHit hit;
        Vector3 origin = transform.position + Vector3.up * 0.5f;

        if (Physics.Raycast(origin, dir, out hit, detectionRange))
        {
            return hit.transform == player;
        }

        return false;
    }

    public bool IsPlayerLookingAtMe()
    {
        Vector3 dir = (transform.position - player.position).normalized;
        float dot = Vector3.Dot(player.forward, dir);
        return dot > 0.5f;
    }

    public void SetDarkZone(bool value)
    {
        playerInDarkZone = value;
        if (value) ScheduleTeleport();
    }

    public bool IsInDarkZone() => playerInDarkZone;

    public void ScheduleTeleport()
    {
        if (teleportScheduled) return;
        teleportScheduled = true;
        teleportTimer = Random.Range(1f, 3f);
    }

    void HandleTeleportTimer()
    {
        if (!teleportScheduled) return;

        teleportTimer -= Time.deltaTime;

        if (teleportTimer <= 0f)
        {
            teleportScheduled = false;

            if (playerInDarkZone)
            {
                TeleportBehindPlayer();
                ChangeState(new ChaseState(this));
            }
        }
    }

    public void TeleportBehindPlayer()
    {
        navAgent.Warp(player.position - player.forward * 2f);
    }

    public void MoveTowards(Vector3 target)
    {
        navAgent.isStopped = false;
        navAgent.SetDestination(target);
    }

    public void StopMoving()
    {
        navAgent.isStopped = true;
    }

    public void LookAtPlayer()
    {
        Vector3 dir = player.position - transform.position;
        dir.y = 0;

        if (dir != Vector3.zero)
        {
            Quaternion rot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, 5f * Time.deltaTime);
        }
    }
}