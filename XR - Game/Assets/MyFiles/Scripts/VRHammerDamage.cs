using UnityEngine;

public class VRHammerDamage : MonoBehaviour
{
    // This script is now purely a placeholder on your hammer head
    // to confirm that the physics engine links the objects correctly.
    // The actual hit detection is safely handled by BatteryController.cs using Triggers.

    private void Start()
    {
        // Double check if the user assigned the tag correctly in the inspector
        if (!gameObject.CompareTag("Hammer"))
        {
            Debug.LogWarning("WARNING: " + gameObject.name + " is missing the 'Hammer' tag! Please assign it in the Inspector.");
        }
    }
}
