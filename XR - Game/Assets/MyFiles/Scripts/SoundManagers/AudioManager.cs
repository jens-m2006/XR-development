using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio; // Nodig voor de Audio Mixer

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    private AudioSource oneShotSource;
    private AudioSource musicSource;
    private AudioSource ambientSource;

    [Header("Music")]
    [SerializeField] private AudioClip backgroundMusic;
    [SerializeField] private bool playMusicOnStart = true;

    [Header("Ambient")]
    [SerializeField] private AudioClip[] ambientClips;
    [SerializeField] private bool playAmbientOnStart = true;
    [SerializeField, Range(0f, 1f)] private float ambientVolume = 1f;
    [SerializeField] private float minAmbientInterval = 10f;
    [SerializeField] private float maxAmbientInterval = 30f;

    [Header("Audio Mixer Settings")]
    [SerializeField] private AudioMixerGroup sfxMixerGroup;

    private List<AudioSource> sfxLoopSources = new List<AudioSource>();

    [SerializeField, Range(0f, 1f)] private float musicVolume = 1f;
    [SerializeField, Range(0f, 1f)] private float sfxVolume = 1f;

    private Queue<AudioClip> ambientQueue = new Queue<AudioClip>();
    private bool isPlayingAmbient = false;
    private Coroutine ambientRoutine;

    private bool sfxEnabled = true;
    private bool musicEnabled = true;

    // Controls whether SFX playback is allowed by game state (e.g. only in LevelPlay)
    private bool allowSfxPlayback = true;

    void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); return; }

        oneShotSource = gameObject.AddComponent<AudioSource>();
        ambientSource = gameObject.AddComponent<AudioSource>();
        musicSource = gameObject.AddComponent<AudioSource>();

        musicSource.loop = true;
        musicSource.playOnAwake = false;
        if (backgroundMusic != null)
        {
            musicSource.clip = backgroundMusic;
        }

        ambientSource.loop = false;
        ambientSource.playOnAwake = false;

        if (sfxMixerGroup != null)
        {
            oneShotSource.outputAudioMixerGroup = sfxMixerGroup;
            ambientSource.outputAudioMixerGroup = sfxMixerGroup;
        }

        musicSource.volume = musicVolume;
        oneShotSource.volume = sfxVolume;
        ambientSource.volume = ambientVolume;
    }

    void Start()
    {
        sfxEnabled = PlayerPrefs.GetInt("SFX", 1) == 1;
        musicEnabled = PlayerPrefs.GetInt("Music", 1) == 1;

        if (playMusicOnStart && musicEnabled && backgroundMusic != null)
        {
            UpdateMusicPlayback(true);
        }

        if (playAmbientOnStart && ambientClips != null && ambientClips.Length > 0)
        {
            StartAmbientRandomPlayback();
        }
    }

    public void RegisterSFXLoop(AudioSource src)
    {
        if (!sfxLoopSources.Contains(src))
            sfxLoopSources.Add(src);
    }

    public void UnregisterSFXLoop(AudioSource src)
    {
        sfxLoopSources.Remove(src);
    }

    // User preference for SFX (saved)
    public void SetSFX(bool enabled)
    {
        sfxEnabled = enabled;
        PlayerPrefs.SetInt("SFX", enabled ? 1 : 0);

        foreach (var src in sfxLoopSources)
        {
            if (src == null) continue;
            if (enabled && allowSfxPlayback) src.UnPause();
            else src.Pause();
        }
    }

    // User preference for music (saved)
    public void SetMusic(bool enabled)
    {
        musicEnabled = enabled;
        PlayerPrefs.SetInt("Music", enabled ? 1 : 0);

        if (!enabled && musicSource != null)
        {
            musicSource.Pause();
        }
    }

    // Called by GameManager to decide if music should actually play now
    public void UpdateMusicPlayback(bool shouldPlay)
    {
        if (musicSource == null) return;

        if (shouldPlay && musicEnabled)
        {
            if (musicSource.clip == null) return;
            if (musicSource.isPlaying) return;

            musicSource.UnPause();
            if (!musicSource.isPlaying)
            {
                musicSource.Play();
            }
        }
        else
        {
            musicSource.Pause();
        }
    }

    // Called by GameManager to allow/block SFX based on state
    public void UpdateSfxPlayback(bool shouldAllow)
    {
        allowSfxPlayback = shouldAllow;

        foreach (var src in sfxLoopSources)
        {
            if (src == null) continue;
            if (shouldAllow && sfxEnabled) src.UnPause();
            else src.Pause();
        }

        if (!shouldAllow && oneShotSource != null && oneShotSource.isPlaying)
        {
            oneShotSource.Stop();
        }
    }

    public void PlayMusic(AudioClip clip)
    {
        if (clip == null) return;
        musicSource.clip = clip;
        if (musicEnabled)
        {
            musicSource.Play();
        }
    }

    public void PlayAmbient(AudioClip clip)
    {
        if (clip == null) return;
        ambientQueue.Enqueue(clip);
        if (!isPlayingAmbient)
        {
            StartCoroutine(PlayAmbientQueue());
        }
    }

    private IEnumerator PlayAmbientQueue()
    {
        isPlayingAmbient = true;
        while (ambientQueue.Count > 0)
        {
            var clip = ambientQueue.Dequeue();
            if (clip == null) continue;
            ambientSource.PlayOneShot(clip, ambientVolume);
            yield return new WaitForSeconds(clip.length);
        }
        isPlayingAmbient = false;
    }

    public void StartAmbientRandomPlayback()
    {
        if (ambientRoutine != null) return;
        ambientRoutine = StartCoroutine(PlayRandomAmbientSounds());
    }

    public void StopAmbientRandomPlayback()
    {
        if (ambientRoutine == null) return;
        StopCoroutine(ambientRoutine);
        ambientRoutine = null;
    }

    private IEnumerator PlayRandomAmbientSounds()
    {
        while (true)
        {
            float waitTime = Random.Range(minAmbientInterval, maxAmbientInterval);
            yield return new WaitForSeconds(waitTime);
            PlayAmbient(GetRandomAmbientClip());
        }
    }

    private AudioClip GetRandomAmbientClip()
    {
        if (ambientClips == null || ambientClips.Length == 0) return null;
        if (ambientClips.Length == 1) return ambientClips[0];

        int index = Random.Range(0, ambientClips.Length);
        return ambientClips[index];
    }

    public void PlaySFX(AudioClip clip)
    {
        if (!sfxEnabled || !allowSfxPlayback || clip == null) return;
        oneShotSource.PlayOneShot(clip, sfxVolume);
    }

    // Inspector-settable volume getters/setters
    public void SetMusicVolume(float v)
    {
        musicVolume = Mathf.Clamp01(v);
        if (musicSource != null) musicSource.volume = musicVolume;
    }

    public void SetSfxVolume(float v)
    {
        sfxVolume = Mathf.Clamp01(v);
        if (oneShotSource != null) oneShotSource.volume = sfxVolume;
        foreach (var src in sfxLoopSources) if (src != null) src.volume = sfxVolume;
    }

    public void SetAmbientVolume(float v)
    {
        ambientVolume = Mathf.Clamp01(v);
        if (ambientSource != null) ambientSource.volume = ambientVolume;
    }
}
