using Suscraft.Core.VoxelTerrainEngine.Chunks;
using System.Collections.Generic;
using UnityEngine;

namespace Suscraft.Core.VoxelTerrainEngine
{
    public class WorldRenderer : MonoBehaviour
    {
        [SerializeField] private GameObject _chunkPrefab;
        private Queue<ChunkRenderer> _chunkPool = new Queue<ChunkRenderer>();

        public void Clear(WorldData worldData)
        {
            foreach (var item in worldData.chunks.Values)
                Destroy(item.gameObject);

            _chunkPool.Clear();
        }

        public ChunkRenderer RenderChunk(WorldData worldData, Vector3Int position, MeshData meshData)
        {
            ChunkRenderer newChunk = null;

            if (_chunkPool.Count > 0)
            {
                newChunk = _chunkPool.Dequeue();
                newChunk.transform.position = position;
            } 
            else
            {
                GameObject chunkObject = Instantiate(_chunkPrefab, position, Quaternion.identity);
                newChunk = chunkObject.GetComponent<ChunkRenderer>();
            }

            newChunk.InitializeChunk(worldData.chunkDatas[position]);
            newChunk.UpdateChunk(meshData);
            newChunk.gameObject.SetActive(true);

            return newChunk;
        }

        public void RemoveChunk(ChunkRenderer chunkRenderer)
        {
            chunkRenderer.gameObject.SetActive(false);
            _chunkPool.Enqueue(chunkRenderer);
        }
    }
}