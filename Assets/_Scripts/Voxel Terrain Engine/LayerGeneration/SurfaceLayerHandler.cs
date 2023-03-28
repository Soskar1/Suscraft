using Suscraft.Core.VoxelTerrainEngine.Chunks;
using Suscraft.Core.VoxelTerrainEngine.Voxels;
using UnityEngine;

namespace Suscraft.Core.VoxelTerrainEngine.Layers
{
    public class SurfaceLayerHandler : VoxelLayerHandler
    {
        [SerializeField] private VoxelType _surfaceVoxelType;

        protected override bool TryHandling(ChunkData chunkData, Vector3Int position, int surfaceHeightNoise, Vector2Int mapSeedOffset)
        {
            if (position.y == surfaceHeightNoise)
            {
                Chunk.SetVoxel(chunkData, position, _surfaceVoxelType);
                return true;
            }

            return false;
        }
    }
}