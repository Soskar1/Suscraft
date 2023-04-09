using System.Collections.Generic;
using UnityEngine;
using Suscraft.Core.VoxelTerrainEngine.Chunks;
using Suscraft.Core.VoxelTerrainEngine.Voxels;
using System;
using System.Collections;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;

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

        private CancellationTokenSource _taskTokenSource = new CancellationTokenSource();

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

        private void OnDisable() => _taskTokenSource.Cancel();

        public async void GenerateWorld() => await GenerateWorld(Vector3Int.zero);

        private async Task GenerateWorld(Vector3Int position)
        {
            WorldGenerationData worldGenerationData = await Task.Run(() => GetVisiblePositions(position), _taskTokenSource.Token);

            foreach (var pos in worldGenerationData.chunkPositionsToRemove)
                WorldDataHelper.RemoveChunk(this, pos);

            foreach (var pos in worldGenerationData.chunkDataToRemove)
                WorldDataHelper.RemoveChunkData(this, pos);

            ConcurrentDictionary<Vector3Int, ChunkData> dataDictionary = null;

            try
            {
                dataDictionary = await CalculateWorldChunkData(worldGenerationData.chunkDataPositionsToCreate);
            }
            catch (Exception)
            {
                return;
            }

            foreach (var calculatedData in dataDictionary)
                WorldData.chunkDatas.Add(calculatedData.Key, calculatedData.Value);

            ConcurrentDictionary<Vector3Int, MeshData> meshDataDictionary = new ConcurrentDictionary<Vector3Int, MeshData>();
            List<ChunkData> dataToRender = WorldData.chunkDatas.
                Where(keyValuePair => worldGenerationData.chunkPositionsToCreate.Contains(keyValuePair.Key)).
                Select(keyValuePair => keyValuePair.Value).
                ToList();

            try
            {
                meshDataDictionary = await CreateMeshDataAsync(dataToRender);
            }
            catch (Exception)
            {
                return;
            }

            StartCoroutine(CreateChunk(meshDataDictionary));
        }

        private Task<ConcurrentDictionary<Vector3Int, MeshData>> CreateMeshDataAsync(List<ChunkData> dataToRender)
        {
            ConcurrentDictionary<Vector3Int, MeshData> dictionary = new ConcurrentDictionary<Vector3Int, MeshData>();

            return Task.Run(() =>
            {
                foreach (ChunkData data in dataToRender)
                {
                    if (_taskTokenSource.Token.IsCancellationRequested)
                        _taskTokenSource.Token.ThrowIfCancellationRequested();

                    MeshData meshData = Chunk.GetChunkMeshData(data);
                    dictionary.TryAdd(data.WorldPosition, meshData);
                }

                return dictionary;
            }, _taskTokenSource.Token
            );
        }

        private Task<ConcurrentDictionary<Vector3Int, ChunkData>> CalculateWorldChunkData(List<Vector3Int> chunkDataPositionsToCreate)
        {
            ConcurrentDictionary<Vector3Int, ChunkData> dictionary = new ConcurrentDictionary<Vector3Int, ChunkData>();

            return Task.Run(() =>
            {
                foreach (Vector3Int pos in chunkDataPositionsToCreate)
                {
                    if (_taskTokenSource.Token.IsCancellationRequested)
                        _taskTokenSource.Token.ThrowIfCancellationRequested();

                    ChunkData data = new ChunkData(_chunkSize, _chunkHeight, this, pos);
                    ChunkData newData = _terrainGenerator.GenerateChunkData(data, _mapSeedOffset);
                    dictionary.TryAdd(pos, newData);
                }

                return dictionary;
            },
            _taskTokenSource.Token
            );
        }

        private IEnumerator CreateChunk(ConcurrentDictionary<Vector3Int, MeshData> meshDataDictionary)
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

        public async void LoadAdditionalChunksRequest(Transform player)
        {
            await GenerateWorld(Vector3Int.RoundToInt(player.position));
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