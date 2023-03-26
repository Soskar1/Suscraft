using UnityEngine;

namespace Suscraft.Core.VoxelTerrainEngine
{
    [CreateAssetMenu(fileName = "Noise Settings", menuName = "Data/Noise Settings")]
    public class NoiseSettings : ScriptableObject
    {
        public float noiseZoom;
        public int octaves;
        public Vector2Int offset;
        public Vector2Int worldOffset;
        public float persistance;
        public float redistributionModifier;
        public float exponent;
    }
}