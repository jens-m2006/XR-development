using UnityEngine;

public class RenderVisibilityController : MonoBehaviour
{
    [Header("Group 1: Level Objects")]
    public GameObject[] levelObjects;

    [Header("Group 2: Menu Objects")]
    public GameObject[] menuObjects;

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
