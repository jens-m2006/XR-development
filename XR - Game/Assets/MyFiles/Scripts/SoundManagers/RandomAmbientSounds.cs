using UnityEngine;
using System.Collections;

public class RandomAmbientSounds : MonoBehaviour
{
    [SerializeField] private AudioClip[] clips;
    [SerializeField] private float minInterval = 5f;
    [SerializeField] private float maxInterval = 15f;

    private Coroutine routine;
    private int lastIndex = -1;

    void Start()
    {
        StartPlaying();
    }

    public void StartPlaying()
    {
        if (routine != null) return; // loopt al
        routine = StartCoroutine(PlayRandomSounds());
    }

    public void StopPlaying()
    {
        if (routine == null) return;
        StopCoroutine(routine);
        routine = null;
    }

    private IEnumerator PlayRandomSounds()
    {
        while (true)
        {
            float waitTime = Random.Range(minInterval, maxInterval);
            yield return new WaitForSeconds(waitTime);
            AudioManager.Instance.PlaySFX(GetRandomClip());
        }
    }

    private AudioClip GetRandomClip()
    {
        if (clips.Length == 1) return clips[0];
        int index;
        do { index = Random.Range(0, clips.Length); }
        while (index == lastIndex);
        lastIndex = index;
        return clips[index];
    }
}