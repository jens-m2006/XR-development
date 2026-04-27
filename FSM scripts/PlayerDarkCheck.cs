using UnityEngine;

public class PlayerDarkCheck : MonoBehaviour
{
    public Agent agent;

    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<DarkZone>())
        {
            agent.SetDarkZone(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<DarkZone>())
        {
            agent.SetDarkZone(false);
        }
    }
}