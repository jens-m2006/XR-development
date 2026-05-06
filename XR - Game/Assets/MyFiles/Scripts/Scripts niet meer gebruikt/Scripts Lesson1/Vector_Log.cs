using UnityEngine;

public class Vector_Log : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 currentposition = transform.forward;
        Debug.Log(currentposition);

    }
}
