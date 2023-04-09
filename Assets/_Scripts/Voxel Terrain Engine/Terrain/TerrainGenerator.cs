using UnityEngine;
using Suscraft.Core.VoxelTerrainEngine.Chunks;

namespace Suscraft.Core.VoxelTerrainEngine.Terrain
{
    public class TerrainGenerator : MonoBehaviour
    {
        [SerializeField] private BiomeGenerator _biomeGenerator;

        public ChunkData GenerateChunkData(ChunkData data, Vector2Int mapSeedOffset)
        {
            TreeData treeData = _biomeGenerator.GetTreeData(data, mapSeedOffset);
            data.treeData = treeData;

            for (int x = 0; x < data.ChunkSize; ++x)
                for (int z = 0; z < data.ChunkSize; ++z)
                    data = _biomeGenerator.ProcessChunkColumn(data, x, z, mapSeedOffset);

            return data;
        }
    }
}