using System.Collections.Generic;
using UnityEngine;
using Suscraft.Core.VoxelTerrainEngine.Chunks;
using Suscraft.Core.VoxelTerrainEngine.Voxels;

namespace Suscraft.Core.VoxelTerrainEngine
{
    public class World : MonoBehaviour
    {
        [SerializeField] private int _mapSizeInChunks = 6;
        [SerializeField] private int _chunkSize = 16;
        [SerializeField] private int _chunkHeight = 100;

        [SerializeField] private GameObject _chunkPrefab;

        [SerializeField] private TerrainGenerator _terrainGenerator;
        [SerializeField] private Vector2Int _mapSeedOffset;

        private Dictionary<Vector3Int, ChunkData> _chunkDatas = new Dictionary<Vector3Int, ChunkData>();
        private Dictionary<Vector3Int, ChunkRenderer> _chunks = new Dictionary<Vector3Int, ChunkRenderer>();

        public int ChunkSize => _chunkSize;
        public int ChunkHeight => _chunkHeight;

        public void GenerateWorld()
        {
            _chunkDatas.Clear();
            foreach(ChunkRenderer chunk in  _chunks.Values)
                Destroy(chunk.gameObject);

            _chunks.Clear();
            for (int x  = 0; x < _mapSizeInChunks; ++x)
            {
                for (int z = 0; z < _mapSizeInChunks; ++z)
                {
                    ChunkData data = new ChunkData(_chunkSize, _chunkHeight, this, new Vector3Int(x * _chunkSize, 0, z * _chunkSize));
                    //GenerateVoxels(data);
                    ChunkData newData = _terrainGenerator.GenerateChunkData(data, _mapSeedOffset);
                    _chunkDatas.Add(newData.WorldPosition, newData);
                }
            }

            foreach (ChunkData data in _chunkDatas.Values)
            {
                MeshData meshData = Chunk.GetChunkMeshData(data);
                GameObject chunkObject = Instantiate(_chunkPrefab, data.WorldPosition, Quaternion.identity);
                ChunkRenderer chunkRenderer = chunkObject.GetComponent<ChunkRenderer>();
                _chunks.Add(data.WorldPosition, chunkRenderer);
                chunkRenderer.InitializeChunk(data);
                chunkRenderer.UpdateChunk(meshData);
            }
        }

        private void GenerateVoxels(ChunkData data)
        {
            
        }

        public VoxelType GetVoxelFromChunkCoordinates(ChunkData chunkData, int x, int y, int z)
        {
            Vector3Int position = Chunk.ChunkPositionFromVoxelCoordinates(this, x, y, z);
            ChunkData containerChunk = null;

            _chunkDatas.TryGetValue(position, out containerChunk);

            if (containerChunk == null)
                return VoxelType.Nothing;

            Vector3Int voxelInChunkCoordinates = Chunk.GetVoxelInChunkCoordinates(containerChunk, new Vector3Int(x, y, z));
            return Chunk.GetVoxelFromChunkCoordinates(containerChunk, voxelInChunkCoordinates);
        }
    }
}