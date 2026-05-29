using UnityEngine;

public class TeleportPlayer : MonoBehaviour
{
    [Header("Player Settings")]
    public GameObject playerRig;

    [Header("Destination Settings")]
    public Transform destinationTarget;
    public Transform menuTarget; 

    private void OnEnable()
    {
        GameManager.OnLevelStarted += TeleportToLevel;
        GameManager.OnMenuStarted += TeleportToMenu;
    }

    private void OnDisable()
    {
        GameManager.OnLevelStarted -= TeleportToLevel;
        GameManager.OnMenuStarted -= TeleportToMenu;
    }

    public void TeleportToLevel()
    {
        if (playerRig == null || destinationTarget == null)
        {
            return;
        }

        playerRig.transform.position = destinationTarget.position;
        playerRig.transform.rotation = destinationTarget.rotation;
    }

    public void TeleportToMenu()
    {
        if (playerRig == null || menuTarget == null)
        {
            return;
        }

        playerRig.transform.position = menuTarget.position;
        playerRig.transform.rotation = menuTarget.rotation;
    }
}
