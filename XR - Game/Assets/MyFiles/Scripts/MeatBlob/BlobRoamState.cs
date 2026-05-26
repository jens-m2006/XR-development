using UnityEngine;

public class BlobRoamState : State
{
    private MeatBlobAgent bAgent;
    private float roamTimer = 0f;
    private float roamInterval = 5f;

    public BlobRoamState(MeatBlobAgent agent) : base(null) { bAgent = agent; }

    public override void Enter()
    {
        // Visual debug check: Green means normal roaming, Dark Red means enraged roaming
        Renderer rend = bAgent.GetComponent<Renderer>();
        if (rend != null) rend.material.color = bAgent.isEnraged ? Color.black : Color.green;

        roamInterval = bAgent.isEnraged ? 2f : 5f; // Changes destination much faster when angry
        PickRandomDestination();
    }

    public override void Update()
    {
        roamTimer += Time.deltaTime;
        if (roamTimer >= roamInterval)
        {
            PickRandomDestination();
            roamTimer = 0f;
        }
    }

    private void PickRandomDestination()
    {
        if (!bAgent.navAgent.isOnNavMesh) return;

        // Pick a random point within a 15-meter circle around the blob
        Vector3 randomDirection = Random.insideUnitSphere * 15f;
        randomDirection += bAgent.transform.position;
        
        UnityEngine.AI.NavMeshHit hit;
        if (UnityEngine.AI.NavMesh.SamplePosition(randomDirection, out hit, 15f, 1))
        {
            bAgent.navAgent.SetDestination(hit.position);
        }
    }
}
