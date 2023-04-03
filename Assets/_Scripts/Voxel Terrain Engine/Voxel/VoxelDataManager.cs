using System.Collections.Generic;
using UnityEngine;

namespace Suscraft.Core.VoxelTerrainEngine.Voxels
{
    public class VoxelDataManager : MonoBehaviour
    {
        private static float _textureOffset = 0.001f;
        private static float _tileSizeX;
        private static float _tileSizeY;
        private static Dictionary<VoxelType, TextureData> _voxelTextureData = new Dictionary<VoxelType, TextureData>();
        [SerializeField] private VoxelDataSO _textureData;

        public static float TileSizeX => _tileSizeX;
        public static float TileSizeY => _tileSizeY;
        public static float TextureOffset => _textureOffset;
        public static Dictionary<VoxelType, TextureData> VoxelTextureData  => _voxelTextureData;

        private void Awake()
        {
            foreach (var item in _textureData.textureDataList)
            {
                if (_voxelTextureData.ContainsKey(item.voxelType) == false)
                {
                    _voxelTextureData.Add(item.voxelType, item);
                };
            }

            _tileSizeX = _textureData.textureSizeX;
            _tileSizeY = _textureData.textureSizeY;
        }
    }
}