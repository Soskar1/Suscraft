using Suscraft.Core.VoxelTerrainEngine;
using UnityEngine;

namespace Suscraft.Core.ProceduralWorldGeneration
{
    public static class WorldDataHelper
    {
        public static Vector3Int ChunkPositionFromVoxelCoords(World world, Vector3Int position)
        {
            return new Vector3Int
            {
                x = Mathf.FloorToInt(position.x / (float)world.ChunkSize) * world.ChunkSize,
                y = Mathf.FloorToInt(position.y / (float)world.ChunkHeight) * world.ChunkHeight,
                z = Mathf.FloorToInt(position.z / (float)world.ChunkSize) * world.ChunkSize
            };
        }
    }
}