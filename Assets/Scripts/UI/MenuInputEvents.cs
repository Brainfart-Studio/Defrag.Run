using BFTools.Core.EventBus;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class MenuInputEvents : MonoBehaviour
{
    private InputAction selectAction;
    private InputAction clickAction;

    private GameObject previousSelected;

    private void Awake()
    {
        selectAction = InputManager.Instance.GetMenuAction("Select");
        clickAction = InputManager.Instance.GetMenuAction("Click");
        selectAction.performed += OnItemSelected;
        clickAction.performed += OnItemSelected;
    }

    private void OnDestroy()
    {
        selectAction.performed -= OnItemSelected;
        clickAction.performed -= OnItemSelected;
    }

    private void Update()
    {
        GameObject currentSelected = EventSystem.current.currentSelectedGameObject;
        if (currentSelected == previousSelected) return;

        previousSelected = currentSelected;
        if (currentSelected == null) return;

        EventBus<MenuNavigatedEvent>.Fire(new MenuNavigatedEvent { SelectedObject = currentSelected });
    }

    private void OnItemSelected(InputAction.CallbackContext context)
    {
        GameObject selected = EventSystem.current.currentSelectedGameObject;
        if (selected == null) return;

        EventBus<MenuItemSelectedEvent>.Fire(new MenuItemSelectedEvent { SelectedObject = selected });
    }
}