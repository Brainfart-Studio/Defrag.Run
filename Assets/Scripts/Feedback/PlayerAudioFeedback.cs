using BFTools.Core.EventBus;
using UnityEngine;

public class PlayerAudioFeedback : MonoBehaviour
{
    [SerializeField] private AudioClip jumpClip;
    [SerializeField] private AudioClip landClip;
    [SerializeField] private AudioClip deathClip;

    [Header("Jump")]
    [SerializeField] private float jumpPitchJitter = 0.05f;

    [Header("Landing Variation")]
    [Tooltip("Fall speed at/below this maps to the quietest, highest-pitched landing")]
    [SerializeField] private float minLandFallSpeed = 3f;
    [Tooltip("Fall speed at/above this maps to the loudest, lowest-pitched landing")]
    [SerializeField] private float maxLandFallSpeed = 20f;
    [SerializeField] private float minLandVolume = 0.5f;
    [SerializeField] private float maxLandVolume = 1f;
    [SerializeField] private float minLandPitch = 1f;
    [SerializeField] private float maxLandPitch = 0.85f;
    [SerializeField] private float landPitchJitter = 0.05f;

    private void OnEnable()
    {
        EventBus<PlayerJumpedEvent>.Subscribe(OnPlayerJumped);
        EventBus<PlayerLandedEvent>.Subscribe(OnPlayerLanded);
        EventBus<PlayerDeathEvent>.Subscribe(OnPlayerDeath);
    }

    private void OnDisable()
    {
        EventBus<PlayerJumpedEvent>.Unsubscribe(OnPlayerJumped);
        EventBus<PlayerLandedEvent>.Unsubscribe(OnPlayerLanded);
        EventBus<PlayerDeathEvent>.Unsubscribe(OnPlayerDeath);
    }

    private void OnPlayerJumped(PlayerJumpedEvent e)
    {
        float pitch = 1f + Random.Range(-jumpPitchJitter, jumpPitchJitter);
        AudioManager.Instance.PlaySFX(jumpClip, 1f, pitch);
    }

    private void OnPlayerLanded(PlayerLandedEvent e)
    {
        float t = Mathf.InverseLerp(minLandFallSpeed, maxLandFallSpeed, e.FallSpeed);
        float volume = Mathf.Lerp(minLandVolume, maxLandVolume, t);
        float pitch = Mathf.Lerp(minLandPitch, maxLandPitch, t) + Random.Range(-landPitchJitter, landPitchJitter);

        AudioManager.Instance.PlaySFX(landClip, volume, pitch);
    }

    private void OnPlayerDeath(PlayerDeathEvent e)
    {
        AudioManager.Instance.PlaySFX(deathClip);
    }
}