using BFTools.Core.EventBus;
using UnityEngine;

public class SpikeHazard : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.TryGetComponent<PlayerController>(out _)) return;

        EventBus<PlayerDeathEvent>.Fire(new PlayerDeathEvent());
    }
}