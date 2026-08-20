using UnityEngine;

[RequireComponent(typeof(LaserCore))]
public class PulseLaser : MonoBehaviour
{
    [SerializeField] private float onDuration = 1f;
    [SerializeField] private float offDuration = 1f;
    [SerializeField] private float startDelay = 0f;

    private LaserCore laserCore;
    private float timer;
    private float delayTimer;
    private bool isLaserOn;

    private void Awake()
    {
        laserCore = GetComponent<LaserCore>();
    }

    // Reset here rather than Start - this is a pooled hazard reused across chunk
    // spawns via SetActive, so Start would only ever fire once and every later
    // reuse would carry over whatever on/off timer it stopped at last time.
    private void OnEnable()
    {
        ResumeFromStart();
    }

    private void Update()
    {
        if (delayTimer > 0f)
        {
            delayTimer -= Time.deltaTime;
            return;
        }

        timer -= Time.deltaTime;

        if (isLaserOn && timer <= 0f)
        {
            laserCore.ToggleLaser(false);
            isLaserOn = false;
            timer = offDuration;
        }
        else if (!isLaserOn && timer <= 0f)
        {
            laserCore.ToggleLaser(true);
            isLaserOn = true;
            timer = onDuration;
        }
    }

    public void PauseAndReset()
    {
        enabled = false;
        laserCore.ToggleLaser(false);
    }

    public void ResumeFromStart()
    {
        delayTimer = startDelay;
        timer = 0f;
        isLaserOn = false;
        laserCore.ToggleLaser(false);
        enabled = true;
    }
}