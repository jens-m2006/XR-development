using UnityEngine;

public class VRHammerDamage : MonoBehaviour
{
    // This script is now purely a placeholder on your hammer head
    // to confirm that the physics engine links the objects correctly.
    // The actual hit detection is safely handled by BatteryController.cs using Triggers.

    private Vector3 startPosition;
    private Quaternion startRotation;

    private void Start()
    {
        startPosition = transform.position;
        startRotation = transform.rotation;

        // Double check if the user assigned the tag correctly in the inspector
        if (!gameObject.CompareTag("Hammer"))
        {
            Debug.LogWarning("WARNING: " + gameObject.name + " is missing the 'Hammer' tag! Please assign it in the Inspector.");
        }

        GameManager.OnLevelReset += ResetHammerPosition;
    }

    private void OnDestroy()
    {
        GameManager.OnLevelReset -= ResetHammerPosition;
    }

    private void ResetHammerPosition()
    {
        transform.position = startPosition;
        transform.rotation = startRotation;
    }
}
