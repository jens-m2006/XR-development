using UnityEngine;

public class ProximityHaptics : MonoBehaviour
{
    [Header("Doelwit")]
    public Transform playerTransform; // Sleep hier je Player of CenterEyeAnchor in

    [Header("Afstand Instellingen")]
    public float maximaleAfstand = 5f; // Vanaf hoeveel meter de controllers gaan trillen
    public float minimaleAfstand = 0.5f; // De afstand waarop de trilling op zijn hardst is (1.0)

    [Header("Tril Instellingen")]
    [Range(0f, 1f)] public float maxTrilSterkte = 0.8f; // Hoe hard de controller maximaal mag trillen

    void Start()
    {
        // Automatische fallback als je de player bent vergeten te slepen
        if (playerTransform == null && Camera.main != null)
        {
            playerTransform = Camera.main.transform;
        }
    }

    void Update()
    {
        if (playerTransform == null) return;

        // 1. Bereken de actuele afstand tussen dit object (de Empty) en de speler
        float afstand = Vector3.Distance(transform.position, playerTransform.position);

        // 2. Als de speler binnen de maximale afstand is, gaan we trillen
        if (afstand < maximaleAfstand)
        {
            // Inverse Lerp berekent een waarde tussen 0.0 (ver weg) en 1.0 (vlakbij)
            float afstandPercentage = Mathf.InverseLerp(maximaleAfstand, minimaleAfstand, afstand);
            
            // Bereken de actuele sterkte op basis van je ingestelde maximum
            float actueleSterkte = afstandPercentage * maxTrilSterkte;

            // 3. Stuur de trilling live naar de controllers (frequentie zetten we op 1f)
            OVRInput.SetControllerVibration(actueleSterkte, 1f, OVRInput.Controller.All);
        }
        else
        {
            // 4. Buiten bereik? Zet de trilling direct op 0
            OVRInput.SetControllerVibration(0f, 0f, OVRInput.Controller.All);
        }
    }

    private void OnDisable()
    {
        // Veiligheid: stop altijd de trilling als het object wordt uitgezet of de scene stopt
        OVRInput.SetControllerVibration(0f, 0f, OVRInput.Controller.All);
    }
}
