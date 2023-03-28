using UnityEngine;

namespace Suscraft.Core.VoxelTerrainEngine.PerlinNoise
{
    [CreateAssetMenu(fileName = "PerlinNoise Settings", menuName = "Data/PerlinNoise Settings")]
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