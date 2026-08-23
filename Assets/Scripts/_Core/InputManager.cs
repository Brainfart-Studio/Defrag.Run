using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private static InputManager instance;
    public static InputManager Instance => instance;

    [SerializeField] private InputActionAsset inputActions;

    private InputActionMap gameplayMap;
    private InputActionMap menuMap;
    private InputActionMap systemMap;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(transform.root.gameObject);

        gameplayMap = inputActions.FindActionMap("Gameplay");
        menuMap = inputActions.FindActionMap("Menu");
        systemMap = inputActions.FindActionMap("System");

        gameplayMap.Enable();
        systemMap.Enable();
    }

    public void EnableGameplay()
    {
        menuMap.Disable();
        gameplayMap.Enable();
        Debug.Log("Gameplay action map enabled");
    }

    public void EnableMenu()
    {
        gameplayMap.Disable();
        menuMap.Enable();
        Debug.Log("Menu action map enabled");
    }

    public InputAction GetGameplayAction(string actionName)
    {
        return gameplayMap.FindAction(actionName);
    }

    public InputAction GetMenuAction(string actionName)
    {
        return menuMap.FindAction(actionName);
    }

    public InputAction GetSystemAction(string actionName)
    {
        return systemMap.FindAction(actionName);
    }
}