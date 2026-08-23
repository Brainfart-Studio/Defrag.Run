using BFTools.Core.EventBus;
using UnityEngine;

public class MenuAudioFeedback : MonoBehaviour
{
    [SerializeField] private AudioClip navigateClip;
    [SerializeField] private AudioClip selectClip;

    private void OnEnable()
    {
        EventBus<MenuNavigatedEvent>.Subscribe(OnMenuNavigated);
        EventBus<MenuItemSelectedEvent>.Subscribe(OnMenuItemSelected);
    }

    private void OnDisable()
    {
        EventBus<MenuNavigatedEvent>.Unsubscribe(OnMenuNavigated);
        EventBus<MenuItemSelectedEvent>.Unsubscribe(OnMenuItemSelected);
    }

    private void OnMenuNavigated(MenuNavigatedEvent e)
    {
        AudioManager.Instance.PlaySFX(navigateClip);
    }

    private void OnMenuItemSelected(MenuItemSelectedEvent e)
    {
        AudioManager.Instance.PlaySFX(selectClip);
    }
}