using UnityEngine;

namespace Suscraft.Core.VoxelTerrainEngine
{
    public class ChunkData
    {
        private VoxelType[] _voxels;
        private int _chunkSize = 16;
        private int _chunkHeight = 100;
        private World _world;
        private Vector3Int _worldPosition;

        public bool modifiedByPlayer = false;

        public int ChunkSize => _chunkSize;
        public int ChunkHeight => _chunkHeight;

        public ChunkData(int chunkSize, int chunkHeight, World world, Vector3Int worldPosition)
        {
            _chunkSize = chunkSize;
            _chunkHeight = chunkHeight;
            _world = world;
            _worldPosition = worldPosition;
            _voxels = new VoxelType[chunkSize * chunkHeight * chunkSize];
        }
    }
}