using BFTools.Core.EventBus;
using BFTools.Feedback.Haptics;
using UnityEngine;

public class PlayerHapticsTrigger : MonoBehaviour
{
    private void OnEnable()
    {
        EventBus<PlayerDashedEvent>.Subscribe(OnPlayerDashed);
        EventBus<PlayerLandedEvent>.Subscribe(OnPlayerLanded);
        EventBus<PlayerDeathEvent>.Subscribe(OnPlayerDeath);
    }

    private void OnDisable()
    {
        EventBus<PlayerDashedEvent>.Unsubscribe(OnPlayerDashed);
        EventBus<PlayerLandedEvent>.Unsubscribe(OnPlayerLanded);
        EventBus<PlayerDeathEvent>.Unsubscribe(OnPlayerDeath);
    }

    private void OnPlayerDashed(PlayerDashedEvent e)
    {
        EventBus<BFHapticsEvent>.Fire(new BFHapticsEvent { eventName = "Dash" });
    }

    private void OnPlayerLanded(PlayerLandedEvent e)
    {
        EventBus<BFHapticsEvent>.Fire(new BFHapticsEvent { eventName = "Landing" });
    }

    private void OnPlayerDeath(PlayerDeathEvent e)
    {
        EventBus<BFHapticsEvent>.Fire(new BFHapticsEvent { eventName = "Death" });
    }
}