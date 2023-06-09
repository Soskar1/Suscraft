using UnityEngine;
using Suscraft.Core.VoxelTerrainEngine.Chunks;
using Suscraft.Core.VoxelTerrainEngine.Noises;
using Suscraft.Core.VoxelTerrainEngine.Layers;
using System.Collections.Generic;

namespace Suscraft.Core.VoxelTerrainEngine.Terrain
{
    public class BiomeGenerator : MonoBehaviour
    {
        [SerializeField] private VoxelLayerHandler _startLayerHandler;
        [SerializeField] private List<VoxelLayerHandler> _additionalLayerHandlers;

        [SerializeField] private NoiseSettings _biomeNoiseSettings;
        [SerializeField] private DomainWarping _domainWarping;

        [SerializeField] private TreeGenerator _treeGenerator;

        public ChunkData ProcessChunkColumn(ChunkData data, int x, int z, Vector2Int mapSeedOffset)
        {
            _biomeNoiseSettings.worldOffset = mapSeedOffset;
            int groundPosition = GetSurfaceHeightNoise(data.ChunkHeight, data.WorldPosition.x + x, data.WorldPosition.z + z);

            for (int y = data.WorldPosition.y; y < data.WorldPosition.y + data.ChunkHeight; ++y)
                _startLayerHandler.Handle(data, new Vector3Int(x, y, z), groundPosition, mapSeedOffset);

            foreach (var layer in _additionalLayerHandlers)
                layer.Handle(data, new Vector3Int(x, data.WorldPosition.y, z), groundPosition, mapSeedOffset);

            return data;
        }

        public TreeData GetTreeData(ChunkData data, Vector2Int mapSeedOffset)
        {
            if (_treeGenerator == null)
                return new TreeData();

            return _treeGenerator.GenerateTreeData(data, mapSeedOffset);
        }

        private int GetSurfaceHeightNoise(int chunkHeight, int x, int z)
        {
            float terrainHeight = _domainWarping.GenerateDomainNoise(x, z, _biomeNoiseSettings);
            terrainHeight = Noise.Redistribution(terrainHeight, _biomeNoiseSettings);

            int surfaceHeight = Noise.RemapValue01ToInt(terrainHeight, 0, chunkHeight);
            return surfaceHeight;
        }
    }
}