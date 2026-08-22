using UnityEngine;

internal struct TemporalLatticeCellState
{
    public Color color;
    public Vector2 offset;
    public Vector2 velocity;
    public float energy;
    public float phase;
}

// A single grid cell's simulation state. Springs toward its neighbors' offset
// and velocity, drifts with a per-cell oscillator, and remembers a short
// history of its own energy/offset so it can react to its own recent past
// (the "temporal" half of the effect) on top of reacting to its neighbors.
internal sealed class TemporalLatticeCell
{
    private readonly TemporalLatticeLayerConfig config;
    private readonly TemporalLatticeCellState[] history;

    private int historyIndex;

    public TemporalLatticeCellState Current;

    public TemporalLatticeCell(TemporalLatticeLayerConfig config, float x, float y)
    {
        this.config = config;

        history = new TemporalLatticeCellState[Mathf.Max(1, config.HistoryLength)];

        // Seeded directly at its resting-state color (energy/velocity/offset are
        // all zero at construction, so that color is fully known up front)
        // instead of starting at white and lerping toward it over the first few
        // frames - the latter is what caused a white flash across the whole
        // layer the instant it initialized.
        Current = new TemporalLatticeCellState
        {
            color = ComputeTargetColor(0f),
            offset = Vector2.zero,
            velocity = Vector2.zero,
            energy = 0f,
            phase = x * 17.31f + y * 31.73f
        };

        for (int i = 0; i < history.Length; i++)
            history[i] = Current;
    }

    public void Tick(
        float dt,
        float elapsed,
        float neighborEnergy,
        Vector2 neighborOffset,
        Vector2 neighborVelocity,
        float impulse,
        float waveDrive,
        Vector2 playerForce,
        float playerEnergy)
    {
        TemporalLatticeCellState past = SampleHistory(config.HistoryDelay);
        float temporalDelta = Current.energy - past.energy;
        Vector2 temporalOffset = Current.offset - past.offset;

        Current.energy += (neighborEnergy - Current.energy) * config.EnergyPropagation * dt;
        Current.energy += impulse * config.ImpulseStrength * dt;
        Current.energy += waveDrive * config.ContinuousWaveStrength * dt;
        Current.energy += playerEnergy * dt;
        Current.energy += Mathf.Abs(temporalDelta) * config.TemporalEnergyFeedback * dt;
        Current.energy -= Current.energy * config.EnergyDecay * dt;
        Current.energy = Mathf.Clamp01(Current.energy);

        Vector2 springForce = (neighborOffset - Current.offset) * config.NeighborSpring;
        Vector2 neighborVelocityForce = (neighborVelocity - Current.velocity) * config.NeighborVelocityInfluence;
        Vector2 temporalForce = temporalOffset * config.TemporalFeedback;

        float oscillator = Mathf.Sin(elapsed * config.CellOscillationSpeed + Current.phase);
        Vector2 oscillatorDirection = new Vector2(Mathf.Cos(Current.phase), Mathf.Sin(Current.phase));
        Vector2 oscillatorForce = oscillatorDirection * oscillator * Current.energy * config.EnergyDisplacement;

        Current.velocity += (springForce + neighborVelocityForce + temporalForce + oscillatorForce + playerForce) * config.ResponseSpeed * dt;
        Current.velocity *= Mathf.Exp(-config.Damping * dt);
        Current.velocity = Vector2.ClampMagnitude(Current.velocity, config.MaxVelocity);

        Current.offset += Current.velocity * dt;
        Current.offset -= Current.offset * config.OffsetReturn * dt;
        Current.offset = Vector2.ClampMagnitude(Current.offset, config.MaxOffset);

        UpdateColor(dt);
    }

    private void UpdateColor(float dt)
    {
        TemporalLatticeCellState past = SampleHistory(config.HistoryDelay);

        float temporalActivity = Mathf.Clamp01(Mathf.Abs(Current.energy - past.energy) * config.TemporalColorInfluence);
        float velocityActivity = Mathf.Clamp01(Current.velocity.magnitude / Mathf.Max(0.001f, config.MaxVelocity));
        float displacementActivity = Mathf.Clamp01(Current.offset.magnitude / Mathf.Max(0.001f, config.MaxOffset));

        float visualValue = Mathf.Clamp01(
            Current.energy * config.EnergyColorInfluence +
            velocityActivity * config.VelocityColorInfluence +
            displacementActivity * config.DisplacementColorInfluence +
            temporalActivity);

        Color target = ComputeTargetColor(visualValue);

        Current.color = Color.Lerp(Current.color, target, 1f - Mathf.Exp(-config.ColorResponse * dt));
    }

    private Color ComputeTargetColor(float visualValue)
    {
        Color target;
        if (config.BaseGradient != null)
        {
            target = config.BaseGradient.Evaluate(Mathf.Lerp(config.GradientBasePosition, config.GradientEnergyPosition, visualValue));
        }
        else
        {
            target = Color.Lerp(new Color(0.015f, 0.02f, 0.04f, 1f), Color.white, visualValue);
        }

        target *= Mathf.Lerp(config.MinBrightness, config.MaxBrightness, visualValue);
        target.a = Mathf.Lerp(config.MinAlpha, config.MaxAlpha, visualValue);

        return target;
    }

    private TemporalLatticeCellState SampleHistory(float delay)
    {
        if (history.Length == 0) return Current;

        delay = Mathf.Clamp01(delay);
        delay += Mathf.Sin(delay * Mathf.PI * 2f) * config.TemporalNonlinearity;
        delay = Mathf.Clamp01(delay);

        int age = Mathf.RoundToInt(delay * (history.Length - 1));
        int newest = (historyIndex - 1 + history.Length) % history.Length;
        int index = (newest - age + history.Length) % history.Length;

        return history[index];
    }

    public void PushHistory()
    {
        history[historyIndex] = Current;
        historyIndex = (historyIndex + 1) % history.Length;
    }
}