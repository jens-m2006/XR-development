using UnityEngine;

public class TeleportPlayer : MonoBehaviour
{

    // Player object =======================================
    [Header("Player Settings")]
    public GameObject playerRig;

    // Destination settingss ===============================
    
    //Level
    [Header("Destination Settings")]
    public Transform destinationTarget;

    // Menu
    public Vector3 menuPosition = new Vector3(0f, 0f, 0f); 
    public Vector3 menuRotation = new Vector3(0f, 0f, 0f); 


    //Action -> Teleport player to level
    private void OnEnable()
    {
        // Luister naar het OnLevelStarted event van de GameManager
        GameManager.OnLevelStarted += TeleportToDestination;
    }

    private void OnDisable()
    {
        // Altijd netjes afmelden om errors en geheugenlekken te voorkomen!
        GameManager.OnLevelStarted -= TeleportToDestination;
    }



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
