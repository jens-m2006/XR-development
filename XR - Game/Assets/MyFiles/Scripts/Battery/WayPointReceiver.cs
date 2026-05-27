using UnityEngine;

public class WaypointReceiver : MonoBehaviour
{
    [Header("Link to Generator")]
    public BatteryController targetBattery;

    [Header("Status")]
    public bool isWaypointDisabled = false;

    void Start()
    {
        // Zet de antenne aan en luister vlijmscherp naar de Invoke van de batterij
        BatteryController.OnAnyBatteryBroken1 += CheckMyBatteryStatus;
    }

    // Deze methode wordt automatisch afgevuurd zodra er EEN batterij kapot gaat
    private void CheckMyBatteryStatus(BatteryController brokenBattery)
    {
        // Controle: Is de batterij die de Invoke stuurde exact de batterij waar IK aan gelinkt ben?
        if (targetBattery != null && brokenBattery == targetBattery)
        {
            isWaypointDisabled = true;
            
            // Dit bericht zie je vanaf nu ALTIJD direct in de console, zonder hulp van de robot!
            Debug.Log($"💥 [WAYPOINT_TEST] De Invoke is ontvangen! Waypoint '{gameObject.name}' weet dat zijn specifieke batterij kapot is en staat nu op DISABLED.");
        }
    }

    // Deze methode is dadelijk voor de robot om te controleren of hij mag laden
    public bool IsThisWaypointDisabled()
    {
        return isWaypointDisabled;
    }

    private void OnDestroy()
    {
        // Netjes afmelden bij scene herstarts om memory leaks te voorkomen
        BatteryController.OnAnyBatteryBroken1 -= CheckMyBatteryStatus;
    }
}
