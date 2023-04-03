using Suscraft.Core.VoxelTerrainEngine.Chunks;
using UnityEngine;

namespace Suscraft.Core.VoxelTerrainEngine.Layers
{
    public class AirLayerHandler : VoxelLayerHandler
    {
        protected override bool TryHandling(ChunkData chunkData, Vector3Int position, int surfaceHeightNoise, Vector2Int mapSeedOffset)
        {
            if (position.y > surfaceHeightNoise)
            {
                Chunk.SetVoxel(chunkData, position, Voxels.VoxelType.Air);
                return true;
            }

            return false;
        }
    }
}