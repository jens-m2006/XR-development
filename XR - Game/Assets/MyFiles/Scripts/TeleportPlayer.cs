using UnityEngine;

public class TeleportPlayer : MonoBehaviour
{

    [Header("Player Settings")]
    public GameObject playerRig;

    [Header("Destination Settings")]
    public Transform destinationTarget;

    public void TeleportToDestination()
    {
        
        if (playerRig == null || destinationTarget == null)
        {
            Debug.LogError("TeleportPlayer: Please assign Player Rig and Destination Target in the Inspector!");
            return;
        }

        
        playerRig.transform.position = destinationTarget.position;
        playerRig.transform.rotation = destinationTarget.rotation;
        
        Debug.Log("Player successfully teleported to: " + destinationTarget.name);
    }
}
