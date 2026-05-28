using UnityEngine;

public class VRHammerDamage : MonoBehaviour
{
    // This script is now purely a placeholder on your hammer head
    // to confirm that the physics engine links the objects correctly.
    // The actual hit detection is safely handled by BatteryController.cs using Triggers.

    private Transform rootTransform;
    private Transform rootOriginalParent;
    private Vector3 rootLocalPosition;
    private Quaternion rootLocalRotation;

    private void Awake()
    {
        rootTransform = transform.root;
        rootOriginalParent = rootTransform.parent;
        rootLocalPosition = rootTransform.localPosition;
        rootLocalRotation = rootTransform.localRotation;
    }

    private void Start()
    {
        // Double check if the user assigned the tag correctly in the inspector
        if (!gameObject.CompareTag("Hammer"))
        {
            Debug.LogWarning("WARNING: " + gameObject.name + " is missing the 'Hammer' tag! Please assign it in the Inspector.");
        }
    }

    private void OnEnable()
    {
        GameManager.OnLevelReset += ResetHammerPosition;
    }

    private void OnDisable()
    {
        GameManager.OnLevelReset -= ResetHammerPosition;
    }

    private void ResetHammerPosition()
    {
        if (rootTransform == null) return;

        rootTransform.SetParent(rootOriginalParent);
        rootTransform.localPosition = rootLocalPosition;
        rootTransform.localRotation = rootLocalRotation;

        Rigidbody rb = rootTransform.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }
}
