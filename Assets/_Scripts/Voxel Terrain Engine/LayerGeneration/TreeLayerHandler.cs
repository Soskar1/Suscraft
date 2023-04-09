using Suscraft.Core.VoxelTerrainEngine.Chunks;
using Suscraft.Core.VoxelTerrainEngine.Voxels;
using UnityEngine;

namespace Suscraft.Core.VoxelTerrainEngine.Layers
{
    public class TreeLayerHandler : VoxelLayerHandler
    {
        [SerializeField] private int _terrainHeightLimit = 25;
        [SerializeField] private int _treeHeight = 5;

        protected override bool TryHandling(ChunkData chunkData, Vector3Int position, int surfaceHeightNoise, Vector2Int mapSeedOffset)
        {
            if (chunkData.WorldPosition.y < 0)
                return false;

            if (surfaceHeightNoise < _terrainHeightLimit && chunkData.treeData.treePositions.Contains(new Vector2Int(position.x, position.z)))
            {
                Vector3Int chunkCoordinates = Chunk.GetVoxelInChunkCoordinates(chunkData, new Vector3Int(position.x, surfaceHeightNoise, position.z));
                VoxelType type = Chunk.GetVoxelFromChunkCoordinates(chunkData, chunkCoordinates);

                if (type == VoxelType.Grass_Dirt)
                {
                    Chunk.SetVoxel(chunkData, chunkCoordinates, VoxelType.Dirt);
                    for (int i = 1; i < _treeHeight; ++i)
                    {
                        chunkCoordinates.y = surfaceHeightNoise + i;
                        Chunk.SetVoxel(chunkData, chunkCoordinates, VoxelType.TreeTrunk);
                    }
                }
            }

            return false;
        }
    }
}