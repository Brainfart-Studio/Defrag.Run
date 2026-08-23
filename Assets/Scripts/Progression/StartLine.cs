using BFTools.Core.EventBus;
using UnityEngine;

public struct GameStartEvent { }

public class StartLine : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.TryGetComponent<PlayerController>(out _)) return;

        EventBus<GameStartEvent>.Fire(new GameStartEvent());
        gameObject.SetActive(false);
        Debug.Log("Game started");
    }
}