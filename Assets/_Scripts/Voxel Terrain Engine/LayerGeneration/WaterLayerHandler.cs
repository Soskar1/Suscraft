using Suscraft.Core.VoxelTerrainEngine.Chunks;
using Suscraft.Core.VoxelTerrainEngine.Voxels;
using UnityEngine;

namespace Suscraft.Core.VoxelTerrainEngine.Layers
{
    public class WaterLayerHandler : VoxelLayerHandler
    {
        [SerializeField] private int _waterLayer = 1;

        protected override bool TryHandling(ChunkData chunkData, Vector3Int position, int surfaceHeightNoise, Vector2Int mapSeedOffset)
        {
            if (position.y > surfaceHeightNoise && position.y <= _waterLayer)
            {
                Chunk.SetVoxel(chunkData, position, VoxelType.Water);

                if (position.y == surfaceHeightNoise + 1)
                {
                    position.y = surfaceHeightNoise;
                    Chunk.SetVoxel(chunkData, position, VoxelType.Sand);
                }

                return true;
            }

            return false;
        }
    }
}