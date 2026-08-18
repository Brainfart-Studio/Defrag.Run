using BFTools.Core.EventBus;
using BFTools.Feedback.Haptics;
using BFTools.Feedback.Vignette;
using UnityEngine;

public class PlayerFeedbackTrigger : MonoBehaviour
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
        EventBus<BFVignetteEvent>.Fire(new BFVignetteEvent { eventName = "Dash" });
    }

    private void OnPlayerLanded(PlayerLandedEvent e)
    {
        EventBus<BFHapticsEvent>.Fire(new BFHapticsEvent { eventName = "Landing" });
    }

    private void OnPlayerDeath(PlayerDeathEvent e)
    {
        EventBus<BFHapticsEvent>.Fire(new BFHapticsEvent { eventName = "Death" });
        EventBus<BFVignetteEvent>.Fire(new BFVignetteEvent { eventName = "Death" });
    }
}