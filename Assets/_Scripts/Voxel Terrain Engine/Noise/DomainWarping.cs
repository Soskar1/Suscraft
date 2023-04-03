using UnityEngine;

namespace Suscraft.Core.VoxelTerrainEngine.Noises
{
    public class DomainWarping : MonoBehaviour
    {
        [SerializeField] private NoiseSettings _noiseDomainX;
        [SerializeField] private NoiseSettings _noiseDomainY;
        [SerializeField] private int _amplitudeX = 20;
        [SerializeField] private int _amplitudeY = 20;

        public float GenerateDomainNoise(int x, int z, NoiseSettings defaultNoiseSettings)
        {
            Vector2 domainOffset = GenerateDomainOffset(x, z);
            return Noise.OctavePerlin(x + domainOffset.x, z + domainOffset.y, defaultNoiseSettings);
        }

        public Vector2 GenerateDomainOffset(int x, int z)
        {
            float noiseX = Noise.OctavePerlin(x, z, _noiseDomainX) * _amplitudeX;
            float noiseY = Noise.OctavePerlin(x, z, _noiseDomainY) * _amplitudeY;
            return new Vector2(noiseX, noiseY);
        }

        public Vector2Int GenerateDomainOffsetInt(int x, int z) => Vector2Int.RoundToInt(GenerateDomainOffset(x, z));
    }
}