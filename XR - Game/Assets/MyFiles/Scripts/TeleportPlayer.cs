using UnityEngine;

public class TeleportPlayer : MonoBehaviour
{
    // Drag your Camera Rig building block into this slot in the Inspector
    [Header("Player Settings")]
    public GameObject playerRig;

    // Drag an empty GameObject (your spawnpoint/destination) into this slot
    [Header("Destination Settings")]
    public Transform destinationTarget;

    public void TeleportToDestination()
    {
        // Safety check to prevent errors if something is missing
        if (playerRig == null || destinationTarget == null)
        {
            Debug.LogError("TeleportPlayer: Please assign Player Rig and Destination Target in the Inspector!");
            return;
        }

        // Move the player rig to the exact position and rotation of the target
        playerRig.transform.position = destinationTarget.position;
        playerRig.transform.rotation = destinationTarget.rotation;
        
        Debug.Log("Player successfully teleported to: " + destinationTarget.name);
    }
}
