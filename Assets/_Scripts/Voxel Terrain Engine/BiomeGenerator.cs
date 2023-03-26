using UnityEngine;

namespace Suscraft.Core.VoxelTerrainEngine
{
    public class BiomeGenerator : MonoBehaviour
    {
        [SerializeField] private int _waterThreshold = 50;
        [SerializeField] private float _noiseScale = 0.03f;

        public ChunkData ProcessChunkColumn(ChunkData data, int x, int z, Vector2Int mapSeedOffset)
        {
            float noiseValue = Mathf.PerlinNoise((data.WorldPosition.x + x + mapSeedOffset.x) * _noiseScale, 
                (data.WorldPosition.z + z + mapSeedOffset.y) * _noiseScale);
            int groundPosition = Mathf.RoundToInt(noiseValue * data.ChunkHeight);

            for (int y = 0; y < data.ChunkHeight; ++y)
            {
                VoxelType voxelType = VoxelType.Dirt;
                if (y > groundPosition)
                {
                    if (y < _waterThreshold)
                        voxelType = VoxelType.Water;
                    else
                        voxelType = VoxelType.Air;
                }
                else if (y == groundPosition && y < _waterThreshold)
                {
                    voxelType = VoxelType.Sand;
                }
                else if (y == groundPosition)
                {
                    voxelType = VoxelType.Grass_Dirt;
                }

                Chunk.SetVoxel(data, new Vector3Int(x, y, z), voxelType);
            }

            return data;
        }
    }
}