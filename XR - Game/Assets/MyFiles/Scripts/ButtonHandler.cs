using UnityEngine;
using UnityEngine.UI; 

public class ButtonHandler : MonoBehaviour
{
    [Header("UI Elementen")]
    public GameObject canvasDoelObject; 
    public Slider laadSlider; 

    [Header("Instellingen")]
    public float laadTijd = 2.0f; 
    public string menuSceneNaam = "MainMenu"; 

    private float huidigeTijd = 0f;
    private bool isAanHetLaden = false;

    void Start()
    {
        if (canvasDoelObject != null) canvasDoelObject.SetActive(false);
        if (laadSlider != null) laadSlider.value = 0;
    }

    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.currentState == GameManager.GameState.Menu)
        {
            // Als de UI nog openstond terwijl we al in het menu zijn beland, sluiten we deze hier direct af
            if (isAanHetLaden) 
            {
                ResetLaden();
            }
            return;
        }

        if (OVRInput.GetDown(OVRInput.RawButton.B))
        {
            isAanHetLaden = true;
            huidigeTijd = 0f;
            if (canvasDoelObject != null) canvasDoelObject.SetActive(true);
        }

        if (isAanHetLaden && OVRInput.Get(OVRInput.RawButton.B))
        {
            huidigeTijd += Time.deltaTime;
            
            if (laadSlider != null)
            {
                laadSlider.value = huidigeTijd / laadTijd;
            }

            if (huidigeTijd >= laadTijd)
            {
                // Eerst de UI netjes opruimen, dan pas de state omschakelen via de GameManager
                ResetLaden();
                GaNaarMainMenu();
            }
        }

        if (OVRInput.GetUp(OVRInput.RawButton.B))
        {
            ResetLaden();
        }
    }

    void ResetLaden()
    {
        isAanHetLaden = false;
        huidigeTijd = 0f;
        if (laadSlider != null) laadSlider.value = 0;
        if (canvasDoelObject != null) canvasDoelObject.SetActive(false);
    }

    void GaNaarMainMenu()
    {
        GameManager.Instance.CancelLevel();
    }
}
