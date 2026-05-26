using UnityEngine;
using System.Collections;

public class BlobLungeState : State
{
    private MeatBlobAgent bAgent;
    private float lungeDuration = 0.25f; // Supersnelle sprong (250 milliseconden)

    public BlobLungeState(MeatBlobAgent agent) : base(null) 
    { 
        bAgent = agent; 
    }

    public override void Enter()
    {
        Debug.Log("MEATBLOB LUNGE: Enter state active!");

        if (bAgent.playerTransform == null)
        {
            Debug.LogError("ERROR: MeatBlob cannot lunge because 'Player Transform' is empty in the Inspector!");
            return;
        }

        // CRUCIALE AUDIO TOEVOEGING:
        // Speel het jumpscare geluid direct af via jouw AudioManager Singleton
        if (bAgent.lungeSound != null && AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(bAgent.lungeSound);
        }

        // Start de fysieke sprong direct vanaf de MonoBehaviour van de blob zelf
        bAgent.StartCoroutine(ExecutePhysicalLunge());
    }

    private IEnumerator ExecutePhysicalLunge()
    {
        // Schakel de NavMeshAgent direct VOLLEDIG uit om Unity's remmen te omzeilen
        if (bAgent.navAgent != null)
        {
            bAgent.navAgent.velocity = Vector3.zero;
            bAgent.navAgent.isStopped = true;
            bAgent.navAgent.enabled = false;
        }

        Vector3 startPos = bAgent.transform.position;
        Vector3 targetPos = bAgent.playerTransform.position;

        // Draai het gezicht van de blob direct naar de speler toe voor de jumpscare
        bAgent.transform.LookAt(targetPos);

        float elapsedTime = 0f;

        // Forceer de fysieke verplaatsing frame voor frame dwars door alle logica heen
        while (elapsedTime < lungeDuration)
        {
            elapsedTime += Time.deltaTime;
            float percentage = elapsedTime / lungeDuration;

            // Verplaats de blob met brute kracht door de 3D-ruimte
            bAgent.transform.position = Vector3.Lerp(startPos, targetPos, percentage);
            
            yield return null; 
        }

        bAgent.transform.position = bAgent.playerTransform.position;
        Debug.Log("JUMPSCARE: The MeatBlob hit your face!");

        
    }
}
