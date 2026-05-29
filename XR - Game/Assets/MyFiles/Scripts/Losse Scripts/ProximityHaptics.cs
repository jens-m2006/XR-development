using UnityEngine;

public class ProximityHaptics : MonoBehaviour
{
    // Dit maakt een mooi keuzemenu aan in de Unity Inspector
    public enum VibratieType
    {
        Linear,
        Constant
    }

    [Header("Doelwit")]
    public Transform playerTransform; // Sleep hier je Player of CenterEyeAnchor in

    [Header("Type Trilling")]
    public VibratieType typeTrilling = VibratieType.Linear; // Kies hier je gewenste gedrag

    [Header("Afstand Instellingen")]
    public float maximaleAfstand = 5f; // Vanaf hoeveel meter de controllers gaan trillen
    public float minimaleAfstand = 0.5f; // De afstand waarop de trilling op zijn hardst is (bij Linear)

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

        // 1. Bereken de actuele afstand tussen dit object en de speler
        float afstand = Vector3.Distance(transform.position, playerTransform.position);

        // 2. Als de speler binnen de maximale afstand is, gaan we trillen
        if (afstand < maximaleAfstand)
        {
            float actueleSterkte = 0f;

            // Controleer welke keuze is geselecteerd in de Inspector
            if (typeTrilling == VibratieType.Linear)
            {
                // Hoe dichterbij, hoe harder de trilling
                float afstandPercentage = Mathf.InverseLerp(maximaleAfstand, minimaleAfstand, afstand);
                actueleSterkte = afstandPercentage * maxTrilSterkte;
            }
            else if (typeTrilling == VibratieType.Constant)
            {
                // Altijd direct de maximale sterkte zodra je binnen bereik bent
                actueleSterkte = maxTrilSterkte;
            }

            // 3. Stuur de trilling live naar de controllers (frequentie staat op 1f)
            OVRInput.SetControllerVibration(1f, actueleSterkte, OVRInput.Controller.All);
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
