using System.Collections.Generic;
using UnityEngine;
using Suscraft.Core.VoxelTerrainEngine.Chunks;
using Suscraft.Core.VoxelTerrainEngine.Voxels;
using System;

namespace Suscraft.Core.VoxelTerrainEngine
{
    public class World : MonoBehaviour
    {
        [SerializeField] private int _mapSizeInChunks = 6;
        [SerializeField] private int _chunkSize = 16;
        [SerializeField] private int _chunkHeight = 100;
        [SerializeField] private int _chunkDrawingRange = 8;

        [SerializeField] private GameObject _chunkPrefab;

        [SerializeField] private TerrainGenerator _terrainGenerator;
        [SerializeField] private Vector2Int _mapSeedOffset;

        public Action OnWorldGenerated;
        public Action OnNewChunksGenerated;

        public WorldData worldData { get; private set; }

        public int ChunkSize => _chunkSize;
        public int ChunkHeight => _chunkHeight;
        public int ChunkDrawingRange => _chunkDrawingRange;

        private void Awake()
        {
            worldData = new WorldData()
            {
                chunkHeight = _chunkHeight,
                chunkSize = _chunkSize,
                chunkDatas = new Dictionary<Vector3Int, ChunkData>(),
                chunks = new Dictionary<Vector3Int, ChunkRenderer>()
            };
        }

        public void GenerateWorld() => GenerateWorld(Vector3Int.zero);

        private void GenerateWorld(Vector3Int position)
        {
            WorldGenerationData worldGenerationData = GetVisiblePositions(position);

            foreach (var pos in worldGenerationData.chunkPositionsToRemove)
                WorldDataHelper.RemoveChunk(this, pos);

            foreach (var pos in worldGenerationData.chunkDataToRemove)
                WorldDataHelper.RemoveChunkData(this, pos);

            foreach (var pos in worldGenerationData.chunkDataPositionsToCreate)
            {
                ChunkData data = new ChunkData(_chunkSize, _chunkHeight, this, pos);
                ChunkData newData = _terrainGenerator.GenerateChunkData(data, _mapSeedOffset);
                worldData.chunkDatas.Add(pos, newData);
            }

            foreach (var pos in worldGenerationData.chunkPositionsToCreate)
            {
                ChunkData data = worldData.chunkDatas[pos];
                MeshData meshData = Chunk.GetChunkMeshData(data);

                GameObject chunkObject = Instantiate(_chunkPrefab, data.WorldPosition, Quaternion.identity);
                ChunkRenderer chunkRenderer = chunkObject.GetComponent<ChunkRenderer>();
                worldData.chunks.Add(data.WorldPosition, chunkRenderer);

                chunkRenderer.InitializeChunk(data);
                chunkRenderer.UpdateChunk(meshData);
            }

            OnWorldGenerated?.Invoke();
        }

        public void RemoveChunk(ChunkRenderer chunk) => chunk.gameObject.SetActive(false);

        private WorldGenerationData GetVisiblePositions(Vector3Int playerPosition)
        {
            List<Vector3Int> allChunkPositionsNeeded = WorldDataHelper.GetChunkPositionsAroundPlayer(this, playerPosition);
            List<Vector3Int> allChunkDataPositionsNeeded = WorldDataHelper.GetDataPositionsAroundPlayer(this, playerPosition);

            List<Vector3Int> chunkPositionsToCreate = WorldDataHelper.SelectPositionsToCreate(worldData, allChunkPositionsNeeded, playerPosition);
            List<Vector3Int> chunkDataPositionsToCreate = WorldDataHelper.SelectDataPositionsToCreate(worldData, allChunkDataPositionsNeeded, playerPosition);

            List<Vector3Int> chunkPositionsToRemove = WorldDataHelper.GetUnneededChunks(worldData, allChunkPositionsNeeded);
            List<Vector3Int> chunkDataToRemove = WorldDataHelper.GetUnneededData(worldData, allChunkDataPositionsNeeded);

            WorldGenerationData data = new WorldGenerationData
            {
                chunkPositionsToCreate = chunkPositionsToCreate,
                chunkDataPositionsToCreate = chunkDataPositionsToCreate,
                chunkPositionsToRemove = chunkPositionsToRemove,
                chunkDataToRemove = chunkDataToRemove
            };

            return data;
        }

        public VoxelType GetVoxelFromChunkCoordinates(ChunkData chunkData, int x, int y, int z)
        {
            Vector3Int position = Chunk.ChunkPositionFromVoxelCoordinates(this, x, y, z);
            ChunkData containerChunk = null;

            worldData.chunkDatas.TryGetValue(position, out containerChunk);

            if (containerChunk == null)
                return VoxelType.Nothing;

            Vector3Int voxelInChunkCoordinates = Chunk.GetVoxelInChunkCoordinates(containerChunk, new Vector3Int(x, y, z));
            return Chunk.GetVoxelFromChunkCoordinates(containerChunk, voxelInChunkCoordinates);
        }

        public void LoadAdditionalChunksRequest(Transform player)
        {
            Debug.Log("Load more chunks");
            GenerateWorld(Vector3Int.RoundToInt(player.position));
            OnNewChunksGenerated?.Invoke();
        }
    }

    public struct WorldGenerationData
    {
        public List<Vector3Int> chunkPositionsToCreate;
        public List<Vector3Int> chunkDataPositionsToCreate;
        public List<Vector3Int> chunkPositionsToRemove;
        public List<Vector3Int> chunkDataToRemove;
    }

    public struct WorldData
    {
        public int chunkSize;
        public int chunkHeight;
        public Dictionary<Vector3Int, ChunkData> chunkDatas;
        public Dictionary<Vector3Int, ChunkRenderer> chunks;
    }
}