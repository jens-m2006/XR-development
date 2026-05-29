using UnityEngine;
using System.Collections;

public class VRHammerDamage : MonoBehaviour
{
    private Transform rootTransform;
    private Transform rootOriginalParent;
    private Vector3 rootLocalPosition;
    private Quaternion rootLocalRotation;
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable;

    private void Awake()
    {
        rootTransform = transform.root;
        rootOriginalParent = rootTransform.parent;
        rootLocalPosition = rootTransform.localPosition;
        rootLocalRotation = rootTransform.localRotation;

        grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        if (grabInteractable == null)
            grabInteractable = rootTransform.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
    }

    private void Start()
    {
        if (!gameObject.CompareTag("Hammer"))
            Debug.LogWarning("WARNING: " + gameObject.name + " is missing the 'Hammer' tag!");
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
        StartCoroutine(ResetAfterRelease());
    }

    private IEnumerator ResetAfterRelease()
{
    ReleaseAllGrabs();

    // Wacht langer zodat XR toolkit volledig loslaat
    yield return new WaitForSeconds(0.1f);

    if (rootTransform == null) yield break;

    // Disable grab tijdelijk zodat hij niet opnieuw gepakt kan worden tijdens reset
    if (grabInteractable != null)
        grabInteractable.enabled = false;

    rootTransform.SetParent(rootOriginalParent);
    rootTransform.localPosition = rootLocalPosition;
    rootTransform.localRotation = rootLocalRotation;

    Rigidbody rb = rootTransform.GetComponent<Rigidbody>();
    if (rb != null)
    {
        rb.isKinematic = true;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = false;
    }

    // Wacht nog een frame dan grab weer aanzetten
    yield return null;

    if (grabInteractable != null)
        grabInteractable.enabled = true;

    Debug.Log("[HAMMER] Reset voltooid.");
}
    private void ReleaseAllGrabs()
    {
        if (grabInteractable == null) return;

        var selectingInteractors = grabInteractable.interactorsSelecting;
        if (selectingInteractors != null && selectingInteractors.Count > 0)
        {
            var interactorsToRelease = new System.Collections.Generic.List<UnityEngine.XR.Interaction.Toolkit.Interactors.IXRSelectInteractor>(selectingInteractors);
            foreach (var interactor in interactorsToRelease)
            {
                if (interactor != null && grabInteractable.interactionManager != null)
                {
                    grabInteractable.interactionManager.SelectExit(interactor, (UnityEngine.XR.Interaction.Toolkit.Interactables.IXRSelectInteractable)grabInteractable);
                    Debug.Log($"[HAMMER] Force released grab from: {interactor}");
                }
            }
        }
    }
}