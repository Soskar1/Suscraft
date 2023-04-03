using UnityEngine;

namespace Suscraft.Core.VoxelTerrainEngine.Noises
{
    [CreateAssetMenu(fileName = "Noises Settings", menuName = "Data/Noises Settings")]
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