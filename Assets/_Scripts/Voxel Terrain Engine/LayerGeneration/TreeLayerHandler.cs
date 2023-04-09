using Suscraft.Core.VoxelTerrainEngine.Chunks;
using Suscraft.Core.VoxelTerrainEngine.Voxels;
using System.Collections.Generic;
using UnityEngine;

namespace Suscraft.Core.VoxelTerrainEngine.Layers
{
    public class TreeLayerHandler : VoxelLayerHandler
    {
        [SerializeField] private int _terrainHeightLimit = 25;
        [SerializeField] private int _treeHeight = 5;

        private static List<Vector3Int> _treeLeafesStaticLayout = new List<Vector3Int>
        {
            new Vector3Int(-2, 0, -2),
            new Vector3Int(-2, 0, -1),
            new Vector3Int(-2, 0, 0),
            new Vector3Int(-2, 0, 1),
            new Vector3Int(-2, 0, 2),
            new Vector3Int(-1, 0, -2),
            new Vector3Int(-1, 0, -1),
            new Vector3Int(-1, 0, 0),
            new Vector3Int(-1, 0, 1),
            new Vector3Int(-1, 0, 2),
            new Vector3Int(0, 0, -2),
            new Vector3Int(0, 0, -1),
            new Vector3Int(0, 0, 0),
            new Vector3Int(0, 0, 1),
            new Vector3Int(0, 0, 2),
            new Vector3Int(1, 0, -2),
            new Vector3Int(1, 0, -1),
            new Vector3Int(1, 0, 0),
            new Vector3Int(1, 0, 1),
            new Vector3Int(1, 0, 2),
            new Vector3Int(2, 0, -2),
            new Vector3Int(2, 0, -1),
            new Vector3Int(2, 0, 0),
            new Vector3Int(2, 0, 1),
            new Vector3Int(2, 0, 2),
            new Vector3Int(-1, 1, -1),
            new Vector3Int(-1, 1, 0),
            new Vector3Int(-1, 1, 1),
            new Vector3Int(0, 1, -1),
            new Vector3Int(0, 1, 0),
            new Vector3Int(0, 1, 1),
            new Vector3Int(1, 1, -1),
            new Vector3Int(1, 1, 0),
            new Vector3Int(1, 1, 1),
            new Vector3Int(0, 2, 0)
        };

        protected override bool TryHandling(ChunkData chunkData, Vector3Int position, int surfaceHeightNoise, Vector2Int mapSeedOffset)
        {
            if (chunkData.WorldPosition.y < 0)
                return false;

            if (surfaceHeightNoise < _terrainHeightLimit &&
                chunkData.treeData.treePositions.Contains(new Vector2Int(chunkData.WorldPosition.x + position.x, chunkData.WorldPosition.z + position.z)))
            {
                Vector3Int chunkCoordinates = new Vector3Int(position.x, surfaceHeightNoise, position.z);
                VoxelType type = Chunk.GetVoxelFromChunkCoordinates(chunkData, chunkCoordinates);

                if (type == VoxelType.Grass_Dirt)
                {
                    Chunk.SetVoxel(chunkData, chunkCoordinates, VoxelType.Dirt);
                    for (int i = 1; i < _treeHeight; ++i)
                    {
                        chunkCoordinates.y = surfaceHeightNoise + i;
                        Chunk.SetVoxel(chunkData, chunkCoordinates, VoxelType.TreeTrunk);
                    }

                    foreach (Vector3Int leafPosition in _treeLeafesStaticLayout)
                        chunkData.treeData.treeLeafesSolid.Add(new Vector3Int(position.x + leafPosition.x, surfaceHeightNoise + _treeHeight + leafPosition.y, position.z + leafPosition.z));
                }
            }

            return false;
        }
    }
}