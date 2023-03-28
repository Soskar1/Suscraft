using UnityEngine;
using Suscraft.Core.VoxelTerrainEngine.Chunks;
using Suscraft.Core.VoxelTerrainEngine.PerlinNoise;
using Suscraft.Core.VoxelTerrainEngine.Layers;

namespace Suscraft.Core.VoxelTerrainEngine
{
    public class BiomeGenerator : MonoBehaviour
    {
        [SerializeField] private VoxelLayerHandler _startLayerHandler;
        [SerializeField] private NoiseSettings _biomeNoiseSettings;
        [SerializeField] private int _waterThreshold = 50;

        public ChunkData ProcessChunkColumn(ChunkData data, int x, int z, Vector2Int mapSeedOffset)
        {
            _biomeNoiseSettings.worldOffset = mapSeedOffset;
            int groundPosition = GetSurfaceHeightNoise(data.ChunkHeight, data.WorldPosition.x + x, data.WorldPosition.z + z);

            for (int y = 0; y < data.ChunkHeight; ++y)
            {
                _startLayerHandler.Handle(data, new Vector3Int(x, y, z), groundPosition, mapSeedOffset);

                //VoxelType voxelType = VoxelType.Dirt;
                //if (y > groundPosition)
                //{
                //    if (y < _waterThreshold)
                //        voxelType = VoxelType.Water;
                //    else
                //        voxelType = VoxelType.Air;
                //}
                //else if (y == groundPosition && y < _waterThreshold)
                //{
                //    voxelType = VoxelType.Sand;
                //}
                //else if (y == groundPosition)
                //{
                //    voxelType = VoxelType.Grass_Dirt;
                //}

                //Chunk.SetVoxel(data, new Vector3Int(x, y, z), voxelType);
            }

            return data;
        }

        private int GetSurfaceHeightNoise(int chunkHeight, int x, int z)
        {
            float terrainHeight = Noise.OctavePerlin(x, z, _biomeNoiseSettings);
            terrainHeight = Noise.Redistribution(terrainHeight, _biomeNoiseSettings);

            int surfaceHeight = Noise.RemapValue01ToInt(terrainHeight, 0, chunkHeight);
            return surfaceHeight;
        }
    }
}