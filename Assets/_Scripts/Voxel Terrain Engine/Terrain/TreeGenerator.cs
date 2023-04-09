using Suscraft.Core.VoxelTerrainEngine.Chunks;
using Suscraft.Core.VoxelTerrainEngine.Noises;
using UnityEngine;

namespace Suscraft.Core.VoxelTerrainEngine.Terrain
{
    public class TreeGenerator : MonoBehaviour
    {
        [SerializeField] private NoiseSettings _treeNoiseSettings;
        [SerializeField] private DomainWarping _domainWarping;

        public TreeData GenerateTreeData(ChunkData chunkData, Vector2Int mapSeedOffset)
        {
            _treeNoiseSettings.worldOffset = mapSeedOffset;
            TreeData treeData = new TreeData();

            float[,] noiseData = GenerateTreeNoise(chunkData, _treeNoiseSettings);

            treeData.treePositions = DataProcessing.FindLocalMaxima(noiseData, chunkData.WorldPosition.x, chunkData.WorldPosition.z);
            return treeData;
        }

        private float[,] GenerateTreeNoise(ChunkData chunkData, NoiseSettings treeNoiseSettings)
        {
            float[,] noiseMax = new float[chunkData.ChunkSize, chunkData.ChunkSize];

            int xMax = chunkData.WorldPosition.x + chunkData.ChunkSize;
            int xMin = chunkData.WorldPosition.x;
            int zMax = chunkData.WorldPosition.z + chunkData.ChunkSize;
            int zMin = chunkData.WorldPosition.z;

            int xIndex = 0;
            int zIndex = 0;
            for (int x = xMin; x < xMax; ++x)
            {
                for (int z = zMin;  z < zMax; ++z)
                {
                    noiseMax[xIndex, zIndex] = _domainWarping.GenerateDomainNoise(x, z, treeNoiseSettings);
                    ++zIndex;
                }
                ++xIndex;
                zIndex = 0;
            }

            return noiseMax;
        }
    }
}