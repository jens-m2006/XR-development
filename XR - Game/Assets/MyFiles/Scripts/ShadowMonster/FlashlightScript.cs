using UnityEngine;

public class FlashlightBeam : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Finding de agent component
        ShadowMonsterAgent shadowMonster = other.GetComponent<ShadowMonsterAgent>();

        // if found let it retreat
        if (shadowMonster != null)
        {
            shadowMonster.TriggerFlashlightHit();
        }
    }
}