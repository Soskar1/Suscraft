using Suscraft.Core.VoxelTerrainEngine.Chunks;
using UnityEngine;

namespace Suscraft.Core.VoxelTerrainEngine.Layers
{
    public abstract class VoxelLayerHandler : MonoBehaviour
    {
        [SerializeField] private VoxelLayerHandler _next;

        public bool Handle(ChunkData chunkData, Vector3Int position, int surfaceHeightNoise, Vector2Int mapSeedOffset)
        {
            if (TryHandling(chunkData, position, surfaceHeightNoise, mapSeedOffset))
                return true;
            if (_next != null)
                return _next.Handle(chunkData, position, surfaceHeightNoise, mapSeedOffset);

            return false;
        }

        protected abstract bool TryHandling(ChunkData chunkData, Vector3Int position, int surfaceHeightNoise, Vector2Int mapSeedOffset);
    }
}