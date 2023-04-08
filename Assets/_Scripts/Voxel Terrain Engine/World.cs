using System.Collections.Generic;
using UnityEngine;
using Suscraft.Core.VoxelTerrainEngine.Chunks;
using Suscraft.Core.VoxelTerrainEngine.Voxels;
using System;
using System.Collections;

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

        private bool _isWorldCreated;

        public WorldData WorldData { get; private set; }

        public int ChunkSize => _chunkSize;
        public int ChunkHeight => _chunkHeight;
        public int ChunkDrawingRange => _chunkDrawingRange;

        private void Awake()
        {
            WorldData = new WorldData()
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
                WorldData.chunkDatas.Add(pos, newData);
            }

            Dictionary<Vector3Int, MeshData> meshDataDictionary = new Dictionary<Vector3Int, MeshData>();
            foreach (Vector3Int pos in worldGenerationData.chunkPositionsToCreate)
            {
                ChunkData data = WorldData.chunkDatas[pos];
                MeshData meshData = Chunk.GetChunkMeshData(data);
                meshDataDictionary.Add(pos, meshData);
            }

            StartCoroutine(CreateChunk(meshDataDictionary));
        }

        private IEnumerator CreateChunk(Dictionary<Vector3Int, MeshData> meshDataDictionary)
        {
            foreach (var item in meshDataDictionary)
            {
                CreateChunk(WorldData, item.Key, item.Value);
                yield return new WaitForEndOfFrame();
            }

            if (!_isWorldCreated)
            {
                _isWorldCreated = true;
                OnWorldGenerated?.Invoke();
            }
        }

        private void CreateChunk(WorldData worldData, Vector3Int position, MeshData meshData)
        {
            GameObject chunkObject = Instantiate(_chunkPrefab, position, Quaternion.identity);
            ChunkRenderer chunkRenderer = chunkObject.GetComponent<ChunkRenderer>();
            WorldData.chunks.Add(position, chunkRenderer);
            chunkRenderer.InitializeChunk(worldData.chunkDatas[position]);
            chunkRenderer.UpdateChunk(meshData);
        }

        public bool SetVoxel(RaycastHit hit, VoxelType voxelType)
        {
            ChunkRenderer chunkRenderer = hit.collider.GetComponent<ChunkRenderer>();
            if (chunkRenderer == null)
                return false;

            Vector3Int pos = GetVoxelPosition(hit);

            WorldDataHelper.SetVoxel(this, pos, voxelType);
            chunkRenderer.ModifiedByPlayer = true;

            if (Chunk.IsOnEdge(chunkRenderer.ChunkData, pos))
            {
                List<ChunkData> neighbourDataList = Chunk.GetEdgeNeighbourChunk(chunkRenderer.ChunkData, pos);
                foreach (ChunkData neighbourData in neighbourDataList)
                {
                    ChunkRenderer chunkToUpdate = WorldDataHelper.GetChunk(neighbourData.World, neighbourData.WorldPosition);
                    if (chunkToUpdate != null)
                        chunkToUpdate.UpdateChunk();
                }
            }

            chunkRenderer.UpdateChunk();

            return true;
        }

        private Vector3Int GetVoxelPosition(RaycastHit hit)
        {
            Vector3 pos = new Vector3(
                GetVoxelPositionIn(hit.point.x, hit.normal.x),
                GetVoxelPositionIn(hit.point.y, hit.normal.y),
                GetVoxelPositionIn(hit.point.z, hit.normal.z)
                );

            return Vector3Int.RoundToInt(pos);
        }

        private float GetVoxelPositionIn(float pos, float normal)
        {
            if (Mathf.Abs(pos % 1) == 0.5f)
                pos -= (normal) / 2;

            return (float)pos;
        }

        public void RemoveChunk(ChunkRenderer chunk) => chunk.gameObject.SetActive(false);

        private WorldGenerationData GetVisiblePositions(Vector3Int playerPosition)
        {
            List<Vector3Int> allChunkPositionsNeeded = WorldDataHelper.GetChunkPositionsAroundPlayer(this, playerPosition);
            List<Vector3Int> allChunkDataPositionsNeeded = WorldDataHelper.GetDataPositionsAroundPlayer(this, playerPosition);

            List<Vector3Int> chunkPositionsToCreate = WorldDataHelper.SelectPositionsToCreate(WorldData, allChunkPositionsNeeded, playerPosition);
            List<Vector3Int> chunkDataPositionsToCreate = WorldDataHelper.SelectDataPositionsToCreate(WorldData, allChunkDataPositionsNeeded, playerPosition);

            List<Vector3Int> chunkPositionsToRemove = WorldDataHelper.GetUnneededChunks(WorldData, allChunkPositionsNeeded);
            List<Vector3Int> chunkDataToRemove = WorldDataHelper.GetUnneededData(WorldData, allChunkDataPositionsNeeded);

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

            WorldData.chunkDatas.TryGetValue(position, out containerChunk);

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