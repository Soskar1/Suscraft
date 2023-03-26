using UnityEngine;

namespace Suscraft.Core.VoxelTerrainEngine
{
    public class TerrainGenerator : MonoBehaviour
    {
        [SerializeField] private BiomeGenerator _biomeGenerator;

        public ChunkData GenerateChunkData(ChunkData data, Vector2Int mapSeedOffset)
        {
            for (int x = 0; x < data.ChunkSize; ++x)
                for (int z = 0; z < data.ChunkSize; ++z)
                    data = _biomeGenerator.ProcessChunkColumn(data, x, z, mapSeedOffset);

            return data;
        }
    }
}