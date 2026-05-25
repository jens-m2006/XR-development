using UnityEngine;

public class LoopingSound : MonoBehaviour
{
    [SerializeField] private bool playOnStart = true;

    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.loop = true;
    }

    void Start()
    {
        AudioManager.Instance.RegisterSFXLoop(audioSource);

        if (playOnStart)
            audioSource.Play();
    }

    void OnDestroy()
    {
        AudioManager.Instance.UnregisterSFXLoop(audioSource);
    }
}