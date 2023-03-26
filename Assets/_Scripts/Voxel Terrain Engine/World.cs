using System;
using System.Collections.Generic;
using UnityEngine;

namespace Suscraft.Core.VoxelTerrainEngine
{
    public class World : MonoBehaviour
    {
        [SerializeField] private int _mapSizeInChunks = 6;
        [SerializeField] private int _chunkSize = 16;
        [SerializeField] private int _chunkHeight = 100;
        [SerializeField] private int _waterThreshold = 50;
        [SerializeField] private float _noiseScale = 0.03f;
        [SerializeField] private GameObject _chunkPrefab;

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
                    GenerateVoxels(data);
                    _chunkDatas.Add(data.WorldPosition, data);
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
            for (int x = 0; x < data.ChunkSize; ++x)
            {
                for (int z = 0; z < data.ChunkSize; ++z)
                {
                    float noiseValue = Mathf.PerlinNoise((data.WorldPosition.x + x) * _noiseScale, (data.WorldPosition.z + z) * _noiseScale);
                    int groundPosition = Mathf.RoundToInt(noiseValue * _chunkHeight);

                    for (int y = 0; y < _chunkHeight; ++y)
                    {
                        VoxelType voxelType = VoxelType.Dirt;
                        if (y > groundPosition)
                        {
                            if (y < _waterThreshold)
                                voxelType = VoxelType.Water;
                            else
                                voxelType = VoxelType.Air;
                        }
                        else if (y == groundPosition)
                        {
                            voxelType = VoxelType.Grass_Dirt;
                        }

                        Chunk.SetVoxel(data, new Vector3Int(x, y, z), voxelType);
                    }
                }
            }
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