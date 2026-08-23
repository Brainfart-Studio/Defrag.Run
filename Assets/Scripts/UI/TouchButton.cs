using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class TouchButton : MonoBehaviour, IPointerDownHandler
{
    public event Action OnPressed;

    public void OnPointerDown(PointerEventData eventData)
    {
        OnPressed?.Invoke();
    }
}