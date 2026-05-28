using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class AutomaticObjectCuller : MonoBehaviour
{
    [System.Serializable]
    public class ManagedComponents
    {
        public GameObject rootObject;
        public List<Renderer> renderers = new List<Renderer>();
        public List<Animator> animators = new List<Animator>();
        public List<MonoBehaviour> scripts = new List<MonoBehaviour>();
        public List<Canvas> canvases = new List<Canvas>();
        public List<MonoBehaviour> ovrCanvases = new List<MonoBehaviour>(); // Voor OVROverlayCanvas
    }

    [Header("Instellingen")]
    [Tooltip("Binnen deze afstand (in meters) blijft het object ALTIJD aan staan.")]
    [SerializeField] private float safeDistance = 6f;
    
    [Tooltip("Hoe vaak per seconde moeten we de visibiliteit controleren? (Lager = beter voor CPU)")]
    [SerializeField] private float checksPerSecond = 5f;

    [Header("Normale Objecten (Alles gaat uit)")]
    [SerializeField] private GameObject[] targetsToManage;

    [Header("Vijanden (Scripts blijven AAN, Visueel/UI/Animator gaat UIT)")]
    [SerializeField] private GameObject[] enemyTargets;

    private Camera mainCamera;
    private List<ManagedComponents> cachedNormalObjects = new List<ManagedComponents>();
    private List<ManagedComponents> cachedEnemyObjects = new List<ManagedComponents>();

    void Start()
    {
        mainCamera = Camera.main;

        CacheComponents(targetsToManage, cachedNormalObjects, isEnemy: false);
        CacheComponents(enemyTargets, cachedEnemyObjects, isEnemy: true);

        StartCoroutine(CullingLoop());
    }

    IEnumerator CullingLoop()
    {
        float waitTime = 1f / checksPerSecond;

        while (true)
        {
            if (mainCamera != null)
            {
                Vector3 cameraPos = mainCamera.transform.position;

                // 1. Verwerk normale objecten
                int normalCount = cachedNormalObjects.Count;
                for (int i = 0; i < normalCount; i++)
                {
                    var managed = cachedNormalObjects[i];
                    if (managed.rootObject == null) continue;

                    bool state = ShouldBeActive(managed.rootObject, cameraPos);
                    
                    ToggleListState(managed.renderers, state);
                    ToggleListState(managed.animators, state);
                    ToggleListState(managed.scripts, state);
                    ToggleListState(managed.canvases, state);
                    ToggleListState(managed.ovrCanvases, state); // Zet zware VR UI uit
                }

                // 2. Verwerk vijanden
                int enemyCount = cachedEnemyObjects.Count;
                for (int i = 0; i < enemyCount; i++)
                {
                    var managed = cachedEnemyObjects[i];
                    if (managed.rootObject == null) continue;

                    bool state = ShouldBeActive(managed.rootObject, cameraPos);

                    ToggleListState(managed.renderers, state);
                    ToggleListState(managed.animators, state);
                    ToggleListState(managed.canvases, state);
                    ToggleListState(managed.ovrCanvases, state); // Zet zware VR UI uit
                }
            }

            yield return new WaitForSeconds(waitTime);
        }
    }

    private bool ShouldBeActive(GameObject obj, Vector3 cameraPos)
    {
        float distance = Vector3.Distance(cameraPos, obj.transform.position);
        if (distance <= safeDistance) return true;
        return IsObjectInCameraView(obj);
    }

    private bool IsObjectInCameraView(GameObject obj)
    {
        Vector3 screenPoint = mainCamera.WorldToViewportPoint(obj.transform.position);
        return screenPoint.z > 0 && 
               screenPoint.x >= 0 && screenPoint.x <= 1 && 
               screenPoint.y >= 0 && screenPoint.y <= 1;
    }

    private void CacheComponents(GameObject[] sourceArray, List<ManagedComponents> targetList, bool isEnemy)
    {
        if (sourceArray == null) return;

        foreach (GameObject obj in sourceArray)
        {
            if (obj == null) continue;

            ManagedComponents managed = new ManagedComponents();
            managed.rootObject = obj;

            managed.renderers.AddRange(obj.GetComponentsInChildren<Renderer>(true));
            managed.animators.AddRange(obj.GetComponentsInChildren<Animator>(true));
            managed.canvases.AddRange(obj.GetComponentsInChildren<Canvas>(true));

            // Zoek specifiek naar OVROverlayCanvas via reflectie/naam om errors te voorkomen
            MonoBehaviour[] allScripts = obj.GetComponentsInChildren<MonoBehaviour>(true);
            foreach (MonoBehaviour script in allScripts)
            {
                if (script == null) continue;

                // Als het script een OVROverlayCanvas is, sla hem apart op
                if (script.GetType().Name == "OVROverlayCanvas")
                {
                    managed.ovrCanvases.Add(script);
                }
                // Normale scripts cachen (alleen voor non-enemies)
                else if (!isEnemy && script != this)
                {
                    managed.scripts.Add(script);
                }
            }
            targetList.Add(managed);
        }
    }

    private void ToggleListState<T>(List<T> components, bool state) where T : Component
    {
        int count = components.Count;
        for (int i = 0; i < count; i++)
        {
            if (components[i] == null) continue;

            if (components[i] is Renderer rend)
            {
                if (rend.enabled != state) rend.enabled = state;
            }
            else if (components[i] is Behaviour behav)
            {
                if (behav.enabled != state) behav.enabled = state;
            }
        }
    }
}
