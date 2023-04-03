using Suscraft.Core.VoxelTerrainEngine.Chunks;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Suscraft.Core.VoxelTerrainEngine
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

        public static List<Vector3Int> GetChunkPositionsAroundPlayer(World world, Vector3Int playerPosition)
        {
            int startX = playerPosition.x - world.ChunkDrawingRange * world.ChunkSize;
            int startZ = playerPosition.z - world.ChunkDrawingRange * world.ChunkSize;
            int endX = playerPosition.x + world.ChunkDrawingRange * world.ChunkSize;
            int endZ = playerPosition.z + world.ChunkDrawingRange * world.ChunkSize;

            List<Vector3Int> chunkPositionsToCreate = new List<Vector3Int>();

            for (int x = startX; x <= endX; x += world.ChunkSize)
            {
                for (int z = startZ; z <= endZ; z += world.ChunkSize)
                {
                    Vector3Int chunkPos = ChunkPositionFromVoxelCoords(world, new Vector3Int(x, 0, z));
                    chunkPositionsToCreate.Add(chunkPos);

                    //if (x >= playerPosition.x - world.ChunkSize && x <= playerPosition.x + world.ChunkSize &&
                    //    z >= playerPosition.z - world.ChunkSize && z <= playerPosition.z + world.ChunkSize)
                    //{
                    //    for (int y = -world.ChunkHeight; y >= playerPosition.y - world.ChunkHeight * 2; y -= world.ChunkHeight)
                    //    {
                    //        chunkPos = ChunkPositionFromVoxelCoords(world, new Vector3Int(x, y, z));
                    //        chunkPositionsToCreate.Add(chunkPos);
                    //    }
                    //}
                }
            }

            return chunkPositionsToCreate;
        }

        public static void RemoveChunkData(World world, Vector3Int pos) => world.worldData.chunkDatas.Remove(pos);

        public static void RemoveChunk(World world, Vector3Int pos)
        {
            ChunkRenderer chunk = null;
            if (world.worldData.chunks.TryGetValue(pos, out chunk))
            {
                world.RemoveChunk(chunk);
                world.worldData.chunks.Remove(pos);
            }
        }

        public static List<Vector3Int> GetDataPositionsAroundPlayer(World world, Vector3Int playerPosition)
        {
            int startX = playerPosition.x - (world.ChunkDrawingRange + 1) * world.ChunkSize;
            int startZ = playerPosition.z - (world.ChunkDrawingRange + 1) * world.ChunkSize;
            int endX = playerPosition.x + (world.ChunkDrawingRange + 1) * world.ChunkSize;
            int endZ = playerPosition.z + (world.ChunkDrawingRange + 1) * world.ChunkSize;

            List<Vector3Int> chunkDataPositionsToCreate = new List<Vector3Int>();

            for (int x = startX; x <= endX; x += world.ChunkSize)
            {
                for (int z = startZ; z <= endZ; z += world.ChunkSize)
                {
                    Vector3Int chunkPos = ChunkPositionFromVoxelCoords(world, new Vector3Int(x, 0, z));
                    chunkDataPositionsToCreate.Add(chunkPos);

                    //if (x >= playerPosition.x - world.ChunkSize && x <= playerPosition.x + world.ChunkSize &&
                    //    z >= playerPosition.z - world.ChunkSize && z <= playerPosition.z + world.ChunkSize)
                    //{
                    //    for (int y = -world.ChunkHeight; y >= playerPosition.y - world.ChunkHeight * 2; y -= world.ChunkHeight)
                    //    {
                    //        chunkPos = ChunkPositionFromVoxelCoords(world, new Vector3Int(x, y, z));
                    //        chunkDataPositionsToCreate.Add(chunkPos);
                    //    }
                    //}
                }
            }

            return chunkDataPositionsToCreate;
        }

        public static List<Vector3Int> GetUnneededData(WorldData worldData, List<Vector3Int> allChunkDataPositionsNeeded)
        {
            return worldData.chunkDatas.Keys
                .Where(pos => allChunkDataPositionsNeeded.Contains(pos) == false && worldData.chunkDatas[pos].modifiedByPlayer == false)
                .ToList();
        }

        public static List<Vector3Int> GetUnneededChunks(WorldData worldData, List<Vector3Int> allChunkPositionsNeeded)
        {
            //List<Vector3Int> positionsToRemove = new List<Vector3Int>();
            //foreach (var pos in worldData.chunks.Keys
            //    .Where(pos => allChunkPositionsNeeded.Contains(pos) == false))
            //{
            //    if (worldData.chunks.ContainsKey(pos))
            //    {
            //        positionsToRemove.Add(pos);
            //    }
            //}
            
            return worldData.chunks.Keys
                .Where(pos => allChunkPositionsNeeded.Contains(pos) == false && worldData.chunks.ContainsKey(pos))
                .ToList();

            //return positionsToRemove;
        }

        public static List<Vector3Int> SelectPositionsToCreate(WorldData worldData, List<Vector3Int> allChunkPositionsNeeded, Vector3Int playerPosition)
        {
            return allChunkPositionsNeeded
                .Where(pos => worldData.chunks.ContainsKey(pos) == false)
                .OrderBy(pos => Vector3.Distance(playerPosition, pos))
                .ToList();
        }

        public static List<Vector3Int> SelectDataPositionsToCreate(WorldData worldData, List<Vector3Int> allChunkDataPositionsNeeded, Vector3Int playerPosition)
        {
            return allChunkDataPositionsNeeded
                .Where(pos => worldData.chunkDatas.ContainsKey(pos) == false)
                .OrderBy(pos => Vector3.Distance(playerPosition, pos))
                .ToList();
        }
    }
}