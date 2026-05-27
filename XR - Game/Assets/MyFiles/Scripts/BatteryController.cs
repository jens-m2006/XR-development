using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BatteryController : MonoBehaviour
{
    public static System.Action OnAnyBatteryBroken; // Creates the radio frequency for the blob
    
    [Header("Battery Settings")]
    public int hitsRequired = 2;

    private Vector3 startPosition;
    private Quaternion startRotation;

    private float currentSpeed = 1f;
    private float slowDownDuration = 2f; 
    private float slowDownTimer = 0f;

    private Dictionary<Light, float> originalIntensities = new Dictionary<Light, float>();
    private Material beamMaterial;

    private int currentHits = 0;
    private bool isBroken = false;
    
    [Header("Components")]
    public Animator componentAnimator;
    public List<Light> statusLights = new List<Light>();

    [Header("Beam Settings")]
    public Renderer beamRenderer; 
    [ColorUsage(true, true)] public Color beamBlueColor = Color.blue; 
    [ColorUsage(true, true)] public Color beamOrangeHitColor = new Color(1f, 0.4f, 0f); 
    [ColorUsage(true, true)] public Color beamRedColor = Color.red;   

    [Header("External Script Component To Disable")]
    public MonoBehaviour scriptOnParentToDisable;

    [Header("Audio Elements (To Disable)")]
    public AudioSource loopAudioSource1;
    public AudioSource loopAudioSource2;

    [Header("Sound Effects (SFX)")]
    public AudioClip hammerHitSound;
    public AudioClip explosionSound;

    


    void Start()
    {
        startPosition = transform.position;
        startRotation = transform.rotation;

        foreach (Light lightSource in statusLights)
        {
            if (lightSource != null)
            {
                originalIntensities[lightSource] = lightSource.intensity;
            }
        }
        
        SetLightsColor(Color.blue);

        if (beamRenderer != null)
        {
            beamMaterial = beamRenderer.material;
            SetBeamEmission(beamBlueColor);
        }

        if (componentAnimator != null)
        {
            componentAnimator.speed = 1f;
        }

        GameManager.OnLevelReset += ResetBattery;
    }

    void Update()
    {
        if (isBroken && currentSpeed > 0f)
        {
            slowDownTimer += Time.deltaTime;
            currentSpeed = Mathf.Lerp(1f, 0f, slowDownTimer / slowDownDuration);
            
            if (componentAnimator != null)
            {
                componentAnimator.speed = currentSpeed;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isBroken) return;

        if (other.CompareTag("Hammer"))
        {
            currentHits++;
            Debug.Log("BATTERY HIT: " + currentHits + " / " + hitsRequired);

            if (currentHits >= hitsRequired)
            {
                if (explosionSound != null && AudioManager.Instance != null)
                {
                    AudioManager.Instance.PlaySFX(explosionSound);
                }

                StartCoroutine(ExplosionAndDisableSequence());
            }
            else
            {
                if (hammerHitSound != null && AudioManager.Instance != null)
                {
                    AudioManager.Instance.PlaySFX(hammerHitSound);
                }

                StartCoroutine(SparkFlashSequence());
            }
        }
    }

    private IEnumerator SparkFlashSequence()
    {
        SetLightsColor(Color.yellow);
        SetBeamEmission(beamOrangeHitColor); 
        
        // Trilt nu 0.4 seconden op 100% kracht zonder errors
        StartCoroutine(VibrateControllersRoutine(0.4f));

        yield return new WaitForSeconds(0.15f); 

        if (!isBroken)
        {
            SetLightsColor(Color.blue);
            SetBeamEmission(beamBlueColor); 
        }
    }

    private IEnumerator ExplosionAndDisableSequence()
    {
        isBroken = true;
        // Fire the signal so the listening MeatBlob goes enraged
        OnAnyBatteryBroken?.Invoke();

        slowDownTimer = 0f;
        Debug.Log("BATTERY DESTROYED: Explosion sequence started!");

        if (scriptOnParentToDisable != null)
        {
            scriptOnParentToDisable.enabled = false;
        }

        if (loopAudioSource1 != null) loopAudioSource1.Stop();
        if (loopAudioSource2 != null) loopAudioSource2.Stop();

        StartCoroutine(VibrateControllersRoutine(1.0f));

        foreach (Light lightSource in statusLights)
        {
            if (lightSource != null)
            {
                lightSource.color = Color.yellow;
                if (originalIntensities.ContainsKey(lightSource))
                {
                    lightSource.intensity = originalIntensities[lightSource] * 8f; 
                }
            }
        }
        
        SetBeamEmission(beamOrangeHitColor); 

        yield return new WaitForSeconds(0.3f); 

        foreach (Light lightSource in statusLights)
        {
            if (lightSource != null)
            {
                lightSource.color = Color.red;
                if (originalIntensities.ContainsKey(lightSource))
                {
                    lightSource.intensity = originalIntensities[lightSource]; 
                }
            }
        }

        SetBeamEmission(beamRedColor);
    }

    private IEnumerator VibrateControllersRoutine(float duration)
    {
        // Zet de constante trilling aan op maximale frequentie (1f) en amplitude (1f)
        OVRInput.SetControllerVibration(1f, 1f, OVRInput.Controller.All);
        
        yield return new WaitForSeconds(duration);
        
        // Stop de trilling volledig
        OVRInput.SetControllerVibration(0f, 0f, OVRInput.Controller.All);
    }

    private void SetLightsColor(Color newColor)
    {
        foreach (Light lightSource in statusLights)
        {
            if (lightSource != null)
            {
                lightSource.color = newColor;
                if (originalIntensities.ContainsKey(lightSource))
                {
                    lightSource.intensity = originalIntensities[lightSource];
                }
            }
        }
    }

    private void SetBeamEmission(Color color)
    {
        if (beamMaterial != null)
        {
            beamMaterial.SetColor("_EmissionColor", color);
            beamMaterial.EnableKeyword("_EMISSION"); 
        }
    }

    private void OnDestroy()
    {
        GameManager.OnLevelReset -= ResetBattery;

        if (beamMaterial != null)
        {
            Destroy(beamMaterial);
        }
    }

    private void OnDisable()
    {
        OVRInput.SetControllerVibration(0f, 0f, OVRInput.Controller.All);
    }

    private void ResetBattery()
    {
        currentHits = 0;
        isBroken = false;
        slowDownTimer = 0f;
        currentSpeed = 1f;

        transform.position = startPosition;
        transform.rotation = startRotation;

        if (componentAnimator != null)
        {
            componentAnimator.speed = 1f;
        }

        if (scriptOnParentToDisable != null)
        {
            scriptOnParentToDisable.enabled = true;
        }

        if (loopAudioSource1 != null && !loopAudioSource1.isPlaying)
        {
            loopAudioSource1.Play();
        }
        if (loopAudioSource2 != null && !loopAudioSource2.isPlaying)
        {
            loopAudioSource2.Play();
        }

        SetLightsColor(Color.blue);
        SetBeamEmission(beamBlueColor);
    }
}
