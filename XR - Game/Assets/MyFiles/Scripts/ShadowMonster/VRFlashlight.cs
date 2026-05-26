using UnityEngine;

public class VRFlashlight : MonoBehaviour
{
    [Header("Flashlight Settings")]
    public float beamRange = 15f;    // How far the light shines
    public float beamRadius = 1.5f;  // The thickness of the light beam

    void Update()
    {
        // Shoot a thick volumetric ray straight forward from the flashlight lens
        RaycastHit hit;
        Vector3 origin = transform.position;
        Vector3 direction = transform.forward;

        // SphereCast acts like a cylinder beam moving through space
        if (Physics.SphereCast(origin, beamRadius, direction, out hit, beamRange))
        {
            // Check if the object we hit has the ShadowMonster component
            ShadowMonsterAgent monster = hit.collider.GetComponent<ShadowMonsterAgent>();
            
            if (monster != null)
            {
                // Tell the monster it got hit by the light
                monster.TriggerFlashlightHit();
            }
        }
    }

    // Visualizes the thickness of the beam inside the Unity Editor scene view
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(transform.position, transform.forward * beamRange);
        Gizmos.DrawWireSphere(transform.position + transform.forward * beamRange, beamRadius);
    }
}
