using UnityEngine;

public class RenderVisibilityController : MonoBehaviour
{
    // The Singleton instance: makes this script globally accessible without drag-and-drop
    public static RenderVisibilityController Instance { get; private set; }

    [Header("Group 1: Level Objects")]
    public GameObject[] levelObjects;

    [Header("Group 2: Menu Objects")]
    public GameObject[] menuObjects;

    private void Awake()
    {
        // Standard Singleton validation check
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void EnableLevel()
    {
        SetGroupState(levelObjects, true);
    }

    public void DisableLevel()
    {
        SetGroupState(levelObjects, false);
    }

    public void EnableMenu()
    {
        SetGroupState(menuObjects, true);
    }

    public void DisableMenu()
    {
        SetGroupState(menuObjects, false);
    }

    private void SetGroupState(GameObject[] list, bool state)
    {
        if (list == null) return;

        foreach (GameObject obj in list)
        {
            if (obj != null)
            {
                obj.SetActive(state);
            }
        }
    }
}
