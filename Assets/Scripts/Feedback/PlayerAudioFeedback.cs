using BFTools.Core.EventBus;
using UnityEngine;

public class PlayerAudioFeedback : MonoBehaviour
{
    [SerializeField] private AudioClip jumpClip;
    [SerializeField] private AudioClip landClip;
    [SerializeField] private AudioClip deathClip;

    [Header("Jump")]
    [SerializeField] private float jumpPitchJitter = 0.05f;

    [Header("Landing")]
    [SerializeField] private float landVolume = 1f;
    [SerializeField] private float landPitch = 1f;
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
        float pitch = landPitch + Random.Range(-landPitchJitter, landPitchJitter);
        AudioManager.Instance.PlaySFX(landClip, landVolume, pitch);
    }

    private void OnPlayerDeath(PlayerDeathEvent e)
    {
        AudioManager.Instance.PlaySFX(deathClip);
    }
}