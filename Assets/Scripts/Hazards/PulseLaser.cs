using BFTools.Core.EventBus;
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
    private bool isDead;

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
        EventBus<GameStateChangedEvent>.Subscribe(OnGameStateChanged);
    }

    private void OnDisable()
    {
        EventBus<GameStateChangedEvent>.Unsubscribe(OnGameStateChanged);
    }

    private void OnGameStateChanged(GameStateChangedEvent e)
    {
        if (e.NewState != GameState.Dying) return;

        isDead = true;
        laserCore.ToggleLaser(false);
    }

    private void Update()
    {
        if (isDead) return;

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
        isDead = false;
        laserCore.ToggleLaser(false);
        enabled = true;
    }
}