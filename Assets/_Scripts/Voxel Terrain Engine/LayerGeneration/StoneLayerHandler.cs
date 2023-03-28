using Suscraft.Core.VoxelTerrainEngine.Chunks;
using Suscraft.Core.VoxelTerrainEngine.Noises;
using UnityEngine;

namespace Suscraft.Core.VoxelTerrainEngine.Layers
{
    public class StoneLayerHandler : VoxelLayerHandler
    {
        [SerializeField][Range(0, 1)] private float _stoneThreshold = 0.5f;

        [SerializeField] private NoiseSettings _stoneNoiseSettings;
        [SerializeField] private DomainWarping _domaingWarping;

        protected override bool TryHandling(ChunkData chunkData, Vector3Int position, int surfaceHeightNoise, Vector2Int mapSeedOffset)
        {
            if (chunkData.WorldPosition.y > surfaceHeightNoise)
                return false;

            _stoneNoiseSettings.worldOffset = mapSeedOffset;
            float stoneNoise = _domaingWarping.GenerateDomainNoise(chunkData.WorldPosition.x + position.x, chunkData.WorldPosition.z + position.z, _stoneNoiseSettings);

            int endPosition = surfaceHeightNoise;
            if (chunkData.WorldPosition.y < 0)
                endPosition = chunkData.WorldPosition.y + chunkData.ChunkHeight;

            if (stoneNoise > _stoneThreshold)
            {
                for (int i = chunkData.WorldPosition.y; i <= endPosition; i++)
                {
                    Vector3Int stonePosition = new Vector3Int(position.x, i, position.z);
                    Chunk.SetVoxel(chunkData, stonePosition, Voxels.VoxelType.Stone);
                }

                return true;
            }

            return false;
        }
    }
}