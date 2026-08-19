using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private static AudioManager instance;
    public static AudioManager Instance => instance;

    private const string MasterVolumeKey = "MasterVolume";
    private const string MusicVolumeKey = "MusicVolume";
    private const string SFXVolumeKey = "SFXVolume";

    [SerializeField] private int sfxPoolSize = 8;

    private AudioSource[] sfxSources;
    private int nextSfxSourceIndex;

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