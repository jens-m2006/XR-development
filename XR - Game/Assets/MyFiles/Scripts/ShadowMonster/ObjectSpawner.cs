using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    [Header("Sleep hier je prefab in")]
    public GameObject prefab;
    public Transform spawnPoint; // leeg GameObject op de startpositie

    private GameObject currentInstance;

    private void Start()
    {
        SpawnObject();
        GameManager.OnLevelReset += RespawnObject;
    }

    private void OnDestroy()
    {
        GameManager.OnLevelReset -= RespawnObject;
    }

    private void RespawnObject()
    {
        if (currentInstance != null)
            Destroy(currentInstance);

        SpawnObject();
    }

    private void SpawnObject()
    {
        currentInstance = Instantiate(prefab, spawnPoint.position, spawnPoint.rotation);
    }
}