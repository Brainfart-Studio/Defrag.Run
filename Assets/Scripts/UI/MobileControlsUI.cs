using UnityEngine;

public class MobileControlsUI : MonoBehaviour
{
    [SerializeField] private GameObject leftControlsRoot;
    [SerializeField] private GameObject rightControlsRoot;

    private void Start()
    {
        bool isMobile = MobileDetector.IsMobile();
        leftControlsRoot.SetActive(isMobile);
        rightControlsRoot.SetActive(isMobile);
    }
}