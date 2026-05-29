using UnityEngine;

public class TeacherLightIndicator : MonoBehaviour
{
    [Header("Connections")]
    // Sleep hier je Drone/Agent object in. Dit praat rechtstreeks met de code.
    public Agent teacherAgent;

    private Light actualLightSource;
    private float customIntensity;

    void Start()
    {
        actualLightSource = GetComponent<Light>();

        // Onthoud de intensiteit die je zelf in de Inspector hebt ingesteld
        if (actualLightSource != null)
        {
            customIntensity = actualLightSource.intensity;
        }
    }

    void Update()
    {
        if (teacherAgent == null || actualLightSource == null) return;

        // Vraagt rechtstreeks de naam van de actieve State op via de methode
        string stateName = teacherAgent.GetCurrentStateName();
        Color stateColor = Color.white;

        // Koppelt de juiste kleur aan de exacte naam van jouw State-bestanden
        if (stateName.Contains("PatrolState")) 
            stateColor = Color.blue;
        else if (stateName.Contains("ChaseState")) 
            stateColor = Color.red;
        else if (stateName.Contains("FleeState")) 
            stateColor = Color.cyan;
        else if (stateName.Contains("IdleState")) 
            stateColor = Color.green;

        // Past de kleur aan met behoud van je eigen intensiteit
        actualLightSource.color = stateColor;
        actualLightSource.intensity = customIntensity;
    }
}
