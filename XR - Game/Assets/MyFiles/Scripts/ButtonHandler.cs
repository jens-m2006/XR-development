using UnityEngine;
using UnityEngine.UI; // Verplicht voor de Slider component
using UnityEngine.SceneManagement; // Verplicht om van scene te wisselen

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
        // voor binnenkort
    }
}
