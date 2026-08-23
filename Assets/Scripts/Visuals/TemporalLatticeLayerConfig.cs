using BFTools.Visuals.Background;
using UnityEngine;

[CreateAssetMenu(fileName = "TemporalLatticeLayerConfig", menuName = "Background/Temporal Lattice Layer")]
public class TemporalLatticeLayerConfig : BFBackgroundLayerConfig
{
    [Header("Grid")]
    [SerializeField, Range(6, 48)] private int columns = 28;
    [SerializeField, Range(4, 32)] private int rows = 16;
    [SerializeField, Range(0f, 0.4f)] private float cellGap = 0.06f;

    [Header("Color")]
    [SerializeField] private Gradient baseGradient;
    [SerializeField, Range(0f, 1f)] private float gradientBasePosition = 0f;
    [SerializeField, Range(0f, 1f)] private float gradientEnergyPosition = 1f;
    [SerializeField, Range(0f, 2f)] private float minBrightness = 0.15f;
    [SerializeField, Range(0f, 3f)] private float maxBrightness = 1.5f;
    [SerializeField, Range(0f, 1f)] private float minAlpha = 0.15f;
    [SerializeField, Range(0f, 1f)] private float maxAlpha = 1f;

    [Header("Temporal Memory")]
    [SerializeField, Range(6, 120)] private int historyLength = 45;
    [SerializeField, Range(0f, 1f)] private float historyDelay = 0.3f;
    [SerializeField, Range(0f, 1f)] private float temporalNonlinearity = 0.5f;
    [SerializeField, Range(0f, 15f)] private float temporalFeedback = 3f;
    [SerializeField, Range(0f, 10f)] private float temporalEnergyFeedback = 2f;

    [Header("Energy Propagation")]
    [SerializeField, Range(0.1f, 10f)] private float energyPropagation = 4f;
    [SerializeField, Range(0f, 5f)] private float energyDecay = 1.2f;

    [Header("Self Generated Activity")]
    [SerializeField, Range(0f, 3f)] private float continuousWaveStrength = 0.35f;
    [SerializeField, Range(0f, 8f)] private float cellOscillationSpeed = 1.2f;
    [SerializeField, Range(0f, 10f)] private float impulseStrength = 3f;
    [SerializeField, Range(0.15f, 5f)] private float impulseInterval = 1.2f;
    [SerializeField, Range(0.03f, 0.6f)] private float impulseRadius = 0.14f;
    [SerializeField, Range(0f, 1f)] private float impulseVariation = 0.9f;

    [Header("Player Interaction")]
    [Tooltip("Tag used to find the player in the scene")]
    [SerializeField] private string playerTag = "Player";
    [Tooltip("Normalized screen-space radius (matches Impulse Radius' scale) within which cells feel the player")]
    [SerializeField, Range(0.03f, 0.6f)] private float playerInteractionRadius = 0.18f;
    [Tooltip("How hard cells get pushed away from the player's screen position")]
    [SerializeField, Range(0f, 30f)] private float playerPushStrength = 12f;
    [Tooltip("How much energy (brightness/activity) the player's presence injects into nearby cells")]
    [SerializeField, Range(0f, 5f)] private float playerEnergyInjection = 2f;

    [Header("Motion")]
    [SerializeField, Range(0.1f, 10f)] private float responseSpeed = 5f;
    [SerializeField, Range(0f, 8f)] private float damping = 2.2f;
    [SerializeField, Range(0f, 10f)] private float neighborSpring = 4f;
    [SerializeField, Range(0f, 8f)] private float neighborVelocityInfluence = 1.5f;
    [SerializeField, Range(1f, 50f)] private float maxVelocity = 16f;
    [SerializeField, Range(1f, 50f)] private float maxOffset = 14f;
    [SerializeField, Range(0f, 8f)] private float offsetReturn = 1.4f;
    [SerializeField, Range(0f, 20f)] private float energyDisplacement = 2.5f;

    [Header("Visual Response")]
    [SerializeField, Range(0f, 5f)] private float energyColorInfluence = 1.5f;
    [SerializeField, Range(0f, 5f)] private float velocityColorInfluence = 0.5f;
    [SerializeField, Range(0f, 5f)] private float displacementColorInfluence = 0.5f;
    [SerializeField, Range(0f, 10f)] private float temporalColorInfluence = 2f;
    [SerializeField, Range(0.1f, 15f)] private float colorResponse = 6f;
    [SerializeField, Range(0f, 1f)] private float energyScale = 0.35f;
    [SerializeField, Range(0f, 1f)] private float velocityScale = 0.15f;
    [SerializeField, Range(0f, 90f)] private float rotationAmount = 20f;

    internal int Columns => columns;
    internal int Rows => rows;
    internal float CellGap => cellGap;

    internal Gradient BaseGradient => baseGradient;
    internal float GradientBasePosition => gradientBasePosition;
    internal float GradientEnergyPosition => gradientEnergyPosition;
    internal float MinBrightness => minBrightness;
    internal float MaxBrightness => maxBrightness;
    internal float MinAlpha => minAlpha;
    internal float MaxAlpha => maxAlpha;

    internal int HistoryLength => historyLength;
    internal float HistoryDelay => historyDelay;
    internal float TemporalNonlinearity => temporalNonlinearity;
    internal float TemporalFeedback => temporalFeedback;
    internal float TemporalEnergyFeedback => temporalEnergyFeedback;

    internal float EnergyPropagation => energyPropagation;
    internal float EnergyDecay => energyDecay;

    internal float ContinuousWaveStrength => continuousWaveStrength;
    internal float CellOscillationSpeed => cellOscillationSpeed;
    internal float ImpulseStrength => impulseStrength;
    internal float ImpulseInterval => impulseInterval;
    internal float ImpulseRadius => impulseRadius;
    internal float ImpulseVariation => impulseVariation;

    internal string PlayerTag => playerTag;
    internal float PlayerInteractionRadius => playerInteractionRadius;
    internal float PlayerPushStrength => playerPushStrength;
    internal float PlayerEnergyInjection => playerEnergyInjection;

    internal float ResponseSpeed => responseSpeed;
    internal float Damping => damping;
    internal float NeighborSpring => neighborSpring;
    internal float NeighborVelocityInfluence => neighborVelocityInfluence;
    internal float MaxVelocity => maxVelocity;
    internal float MaxOffset => maxOffset;
    internal float OffsetReturn => offsetReturn;
    internal float EnergyDisplacement => energyDisplacement;

    internal float EnergyColorInfluence => energyColorInfluence;
    internal float VelocityColorInfluence => velocityColorInfluence;
    internal float DisplacementColorInfluence => displacementColorInfluence;
    internal float TemporalColorInfluence => temporalColorInfluence;
    internal float ColorResponse => colorResponse;
    internal float EnergyScale => energyScale;
    internal float VelocityScale => velocityScale;
    internal float RotationAmount => rotationAmount;

    public override IBFBackgroundLayer CreateLayer()
    {
        return new TemporalLatticeBackgroundLayer(this);
    }
}
