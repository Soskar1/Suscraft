using System;
using UnityEngine;

namespace Suscraft.Core.VoxelTerrainEngine
{
    public static class Chunk
    {
        public static void LoopThroughTheVoxels(ChunkData chunkData, Action<int, int, int> actionToPerform)
        {
            for (int index = 0; index < chunkData.Voxels.Length; ++index)
            {
                var position = GetPositionFromIndex(chunkData, index);
                actionToPerform(position.x, position.y, position.z);
            }
        }

        private static Vector3Int GetPositionFromIndex(ChunkData chunkData, int index)
        {
            int x = index % chunkData.ChunkSize;
            int y = (index / chunkData.ChunkSize) % chunkData.ChunkHeight;
            int z = index / (chunkData.ChunkSize * chunkData.ChunkHeight);
            return new Vector3Int(x, y, z);
        }

        //in chunk coordinate system
        private static bool InRange(ChunkData chunkData, int axisCoordinate)
        {
            if (axisCoordinate < 0 || axisCoordinate >= chunkData.ChunkSize)
                return false;

            return true;
        }

        //in chunk coordinate system (0; ChunkHeight]
        private static bool InRangeHeight(ChunkData chunkData, int yCoordinate)
        {
            if (yCoordinate < 0 || yCoordinate >= chunkData.ChunkHeight)
                return false;

            return true;
        }

        public static void SetVoxel(ChunkData chunkData, Vector3Int localPosition, VoxelType voxel)
        {
            if (InRange(chunkData, localPosition.x) && InRangeHeight(chunkData, localPosition.y) && InRange(chunkData, localPosition.z))
            {
                int index = GetIndexFromPosition(chunkData, localPosition.x, localPosition.y, localPosition.z);
                chunkData.Voxels[index] = voxel;
            } 
            else
            {
                throw new Exception("Need to ask World for appropriate chunk");
            }
        }

        private static int GetIndexFromPosition(ChunkData chunkData, int x, int y, int z)
        {
            return x + chunkData.ChunkSize * y + chunkData.ChunkSize * chunkData.ChunkHeight * z;
        }

        public static Vector3Int GetVoxelInChunkCoordinates(ChunkData chunkData, Vector3Int pos)
        {
            return new Vector3Int
            {
                x = pos.x - chunkData.WorldPosition.x,
                y = pos.y - chunkData.WorldPosition.y,
                z = pos.z - chunkData.WorldPosition.z
            };
        }

        public static VoxelType GetVoxelFromChunkCoordinates(ChunkData chunkData, int x, int y, int z)
        {
            if (InRange(chunkData, x) && InRangeHeight(chunkData, y) && InRange(chunkData, z))
            {
                int index = GetIndexFromPosition(chunkData, x, y, z);
                return chunkData.Voxels[index];
            }

            return chunkData.World.GetVoxelFromChunkCoordinates(chunkData, chunkData.WorldPosition.x + x, chunkData.WorldPosition.y + y, chunkData.WorldPosition.z + z);
        }

        public static VoxelType GetVoxelFromChunkCoordinates(ChunkData chunkData, Vector3Int chunkCoordinates)
        {
            return GetVoxelFromChunkCoordinates(chunkData, chunkCoordinates.x, chunkCoordinates.y, chunkCoordinates.z);
        }

        public static MeshData GetChunkMeshData(ChunkData chunkData)
        {
            MeshData meshData = new MeshData(true);

            LoopThroughTheVoxels(chunkData, (x, y, z) => meshData = VoxelHelper.GetMeshData(chunkData, x, y, z, meshData, chunkData.Voxels[GetIndexFromPosition(chunkData, x, y, z)]));

            return meshData;
        }

        public static Vector3Int ChunkPositionFromVoxelCoordinates(World world, int x, int y, int z)
        {
            return new Vector3Int
            {
                x = Mathf.FloorToInt(x / (float)world.ChunkSize) * world.ChunkSize,
                y = Mathf.FloorToInt(y / (float)world.ChunkHeight) * world.ChunkHeight,
                z = Mathf.FloorToInt(z / (float)world.ChunkSize) * world.ChunkSize
            };
        }
    }
}