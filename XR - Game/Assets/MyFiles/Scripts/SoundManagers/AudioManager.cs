using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    private AudioSource oneShotSource;
    private AudioSource musicSource;
    [SerializeField] private AudioClip backgroundMusic;

    private List<AudioSource> sfxLoopSources = new List<AudioSource>();

    [SerializeField, Range(0f,1f)] private float musicVolume = 1f;
    [SerializeField, Range(0f,1f)] private float sfxVolume = 1f;
    [SerializeField, Range(0f,1f)] private float ambientVolume = 1f;

    private Queue<AudioClip> ambientQueue = new Queue<AudioClip>();
    private bool isPlayingAmbient = false;

    private bool sfxEnabled = true;
    private bool musicEnabled = true;

    // Controls whether SFX playback is allowed by game state (e.g. only in LevelPlay)
    private bool allowSfxPlayback = true;

    void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); return; }

        oneShotSource = gameObject.AddComponent<AudioSource>();
        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.loop = true;
        if (backgroundMusic != null)
        {
            musicSource.clip = backgroundMusic;
        }

        // apply initial volumes
        musicSource.volume = musicVolume;
        oneShotSource.volume = sfxVolume;
    }

    void Start()
    {
        sfxEnabled   = PlayerPrefs.GetInt("SFX",   1) == 1;
        musicEnabled = PlayerPrefs.GetInt("Music", 1) == 1;
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

        // apply to loop sources depending on enabled flag and current allow flag
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

        if (!enabled)
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

        // Stop all SFX loops if not allowed (e.g., in Menu state)
        foreach (var src in sfxLoopSources)
        {
            if (src == null) continue;
            if (shouldAllow && sfxEnabled) src.UnPause();
            else src.Pause();
        }

        // Also stop the one-shot source if transitioning to Menu
        if (!shouldAllow && oneShotSource != null && oneShotSource.isPlaying)
        {
            oneShotSource.Stop();
        }
    }

    public void PlayMusic(AudioClip clip)
    {
        musicSource.clip = clip;
        if (musicEnabled) musicSource.Play();
    }

    // Enqueue ambient clip to play after currently playing ambient finishes
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
            oneShotSource.PlayOneShot(clip, ambientVolume);
            // wait until clip is finished
            yield return new WaitForSeconds(clip.length);
        }
        isPlayingAmbient = false;
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
    }
}