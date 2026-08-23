using UnityEngine;

public class OrientationBlockerUI : MonoBehaviour
{
    [SerializeField] private GameObject panelRoot;

    private void Awake()
    {
        OrientationDetector.OnOrientationChanged += HandleOrientationChanged;
    }

    private void OnDestroy()
    {
        OrientationDetector.OnOrientationChanged -= HandleOrientationChanged;
    }

    private void Start()
    {
        SetBlocked(MobileDetector.IsMobile() && OrientationDetector.Instance.IsCurrentlyPortrait());
    }

    private void HandleOrientationChanged(bool isPortrait)
    {
        SetBlocked(MobileDetector.IsMobile() && isPortrait);
    }

    private void SetBlocked(bool isBlocked)
    {
        panelRoot.SetActive(isBlocked);
        Time.timeScale = isBlocked ? 0f : 1f;
    }
}