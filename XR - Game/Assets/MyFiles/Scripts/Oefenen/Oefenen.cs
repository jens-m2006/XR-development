using UnityEngine;
using Unity.VisualScripting;
using UnityEngine;

public class Oefenen : MonoBehaviour
{
    
    
    [SerializeField] private GameObject object1;
    public float points = 3;

    public GameObject[] enemies;
    public Color playerColor;

    public Vector3 test = new Vector3(1f,1f,1f); // dit is de start locatie

    //test


    //test22

    




    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        object1.transform.position = test;
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
