using UnityEngine;

public class MenuInputActivator : MonoBehaviour
{
    private void Start()
    {
        InputManager.Instance.EnableMenu();
    }
}