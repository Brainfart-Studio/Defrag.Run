using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MusicLayerConfig", menuName = "Audio/Music Layer Config")]
public class MusicLayerConfig : ScriptableObject
{
    [Serializable]
    public class MusicLayer
    {
        public AudioClip clip;

        [Tooltip("Always plays at full volume, unaffected by difficulty or the death fade")]
        public bool isBase;

        [Tooltip("Difficulty at which this layer starts fading in")]
        public float startDifficulty;

        [Tooltip("Difficulty at which this layer reaches its max volume")]
        public float maxDifficulty;

        [Range(0f, 1f)]
        [Tooltip("Volume this layer reaches once past maxDifficulty")]
        public float maxVolume = 1f;
    }

    public List<MusicLayer> layers = new List<MusicLayer>();
}