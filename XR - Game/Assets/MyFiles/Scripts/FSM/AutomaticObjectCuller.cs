using UnityEngine;

public class AutomaticObjectCuller : MonoBehaviour
{
    [Header("Instellingen")]
    [Tooltip("Binnen deze afstand (in meters) blijft het object ALTIJD aan staan.")]
    [SerializeField] private float safeDistance = 6f;

    [Header("Sleep hier alle objecten in die je wilt beheren")]
    [SerializeField] private GameObject[] targetsToManage;

    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (mainCamera == null || targetsToManage == null) return;

        foreach (GameObject target in targetsToManage)
        {
            if (target == null) continue;

            // Bereken de afstand tussen de camera en het object
            float distance = Vector3.Distance(mainCamera.transform.position, target.transform.position);

            bool isInView;

            // Als je heel dichtbij staat, dwingen we het object om AAN te blijven (voor de ooghoeken)
            if (distance <= safeDistance)
            {
                isInView = true; 
            }
            else
            {
                // Ben je verder weg? Dan gebruiken we de normale cameracheck
                isInView = IsObjectInCameraView(target);
            }

            ToggleComponents(target, isInView);
        }
    }

    private bool IsObjectInCameraView(GameObject obj)
    {
        Vector3 screenPoint = mainCamera.WorldToViewportPoint(obj.transform.position);
        return screenPoint.z > 0 && 
               screenPoint.x >= 0 && screenPoint.x <= 1 && 
               screenPoint.y >= 0 && screenPoint.y <= 1;
    }

    private void ToggleComponents(GameObject obj, bool state)
    {
        Animator[] animators = obj.GetComponentsInChildren<Animator>();
        foreach (Animator anim in animators)
        {
            if (anim.enabled != state) 
            {
                anim.enabled = state;
                Debug.Log($"[MY] Animator op {obj.name} staat nu: " + (state ? "AAN" : "UIT"));
            }
        }

        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
        foreach (Renderer rend in renderers)
        {
            if (rend.enabled != state) rend.enabled = state;
        }

        MonoBehaviour[] scripts = obj.GetComponentsInChildren<MonoBehaviour>();
        foreach (MonoBehaviour script in scripts)
        {
            if (script != this && script.enabled != state) script.enabled = state;
        }
    }
}
