using Suscraft.Core.VoxelTerrainEngine.Chunks;
using Suscraft.Core.VoxelTerrainEngine.Voxels;
using UnityEngine;

namespace Suscraft.Core.VoxelTerrainEngine.Layers
{
    public class UndergroundLayerHandler : VoxelLayerHandler
    {
        [SerializeField] private VoxelType _undergroundVoxelType;

        protected override bool TryHandling(ChunkData chunkData, Vector3Int position, int surfaceHeightNoise, Vector2Int mapSeedOffset)
        {
            if (position.y < surfaceHeightNoise)
            {
                Chunk.SetVoxel(chunkData, new Vector3Int(position.x, position.y - chunkData.WorldPosition.y, position.z), _undergroundVoxelType);
                return true;
            }

            return false;
        }
    }
}