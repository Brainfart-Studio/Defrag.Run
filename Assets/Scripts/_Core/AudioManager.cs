using System.Collections.Generic;
using BFTools.Core.EventBus;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private static AudioManager instance;
    public static AudioManager Instance => instance;

    private const string MasterVolumeKey = "MasterVolume";
    private const string MusicVolumeKey = "MusicVolume";
    private const string SFXVolumeKey = "SFXVolume";

    [SerializeField] private int sfxPoolSize = 8;

    [SerializeField] private MusicLayerConfig musicConfig;

    [Tooltip("How fast a music layer's volume glides toward its target, in volume units per second")]
    [SerializeField] private float volumeLerpSpeed = 0.5f;

    private AudioSource[] sfxSources;
    private int nextSfxSourceIndex;

    private AudioSource[] musicLayerSources;

    public float MasterVolume { get; private set; }
    public float MusicVolume { get; private set; }
    public float SFXVolume { get; private set; }

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(transform.root.gameObject);

        MasterVolume = PlayerPrefs.GetFloat(MasterVolumeKey, 1f);
        MusicVolume = PlayerPrefs.GetFloat(MusicVolumeKey, 1f);
        SFXVolume = PlayerPrefs.GetFloat(SFXVolumeKey, 1f);

        sfxSources = new AudioSource[sfxPoolSize];
        for (int i = 0; i < sfxPoolSize; i++)
        {
            sfxSources[i] = gameObject.AddComponent<AudioSource>();
            sfxSources[i].playOnAwake = false;
        }

        BuildMusicLayerSources();

        EventBus<GameStartEvent>.Subscribe(OnGameStart);
    }

    private void OnDestroy()
    {
        EventBus<GameStartEvent>.Unsubscribe(OnGameStart);
    }

    private void Update()
    {
        if (musicLayerSources == null) return;

        float difficulty = DifficultyManager.Instance != null ? DifficultyManager.Instance.Difficulty : 0f;

        for (int i = 0; i < musicLayerSources.Length; i++)
        {
            MusicLayerConfig.MusicLayer layer = musicConfig.layers[i];
            float target = EvaluateLayerVolume(layer, difficulty) * MusicVolume * MasterVolume;
            AudioSource source = musicLayerSources[i];
            source.volume = Mathf.MoveTowards(source.volume, target, volumeLerpSpeed * Time.deltaTime);
        }
    }

    private void OnGameStart(GameStartEvent e)
    {
        BeginMusic();
    }

    private void BuildMusicLayerSources()
    {
        if (musicConfig == null || musicConfig.layers.Count == 0) return;

        musicLayerSources = new AudioSource[musicConfig.layers.Count];
        for (int i = 0; i < musicConfig.layers.Count; i++)
        {
            AudioSource source = gameObject.AddComponent<AudioSource>();
            source.clip = musicConfig.layers[i].clip;
            source.loop = true;
            source.playOnAwake = false;
            source.volume = 0f;
            musicLayerSources[i] = source;
        }
    }

    private void BeginMusic()
    {
        if (musicLayerSources == null) return;

        double startTime = AudioSettings.dspTime + 0.1;
        foreach (AudioSource source in musicLayerSources)
        {
            source.PlayScheduled(startTime);
        }
    }

    private float EvaluateLayerVolume(MusicLayerConfig.MusicLayer layer, float difficulty)
    {
        if (difficulty <= layer.startDifficulty) return 0f;
        if (difficulty >= layer.maxDifficulty) return layer.maxVolume;

        float t = (difficulty - layer.startDifficulty) / (layer.maxDifficulty - layer.startDifficulty);
        return Mathf.Lerp(0f, layer.maxVolume, t);
    }

    public void SetMasterVolume(float value)
    {
        MasterVolume = Mathf.Clamp01(value);
        PlayerPrefs.SetFloat(MasterVolumeKey, MasterVolume);
    }

    public void SetMusicVolume(float value)
    {
        MusicVolume = Mathf.Clamp01(value);
        PlayerPrefs.SetFloat(MusicVolumeKey, MusicVolume);
    }

    public void SetSFXVolume(float value)
    {
        SFXVolume = Mathf.Clamp01(value);
        PlayerPrefs.SetFloat(SFXVolumeKey, SFXVolume);
    }

    public void PlaySFX(AudioClip clip, float volume = 1f)
    {
        if (clip == null) return;

        AudioSource source = sfxSources[nextSfxSourceIndex];
        nextSfxSourceIndex = (nextSfxSourceIndex + 1) % sfxSources.Length;

        source.PlayOneShot(clip, volume * SFXVolume * MasterVolume);
    }
}