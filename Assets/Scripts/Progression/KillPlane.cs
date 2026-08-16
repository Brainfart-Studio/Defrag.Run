using BFTools.Core.EventBus;
using UnityEngine;

public struct PlayerDeathEvent { }

public class KillPlane : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.TryGetComponent<PlayerController>(out _)) return;

        EventBus<PlayerDeathEvent>.Fire(new PlayerDeathEvent());
    }
}