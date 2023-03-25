using System.Collections.Generic;
using UnityEngine;

namespace Suscraft.Core.VoxelTerrainEngine
{
    public class VoxelDataManager : MonoBehaviour
    {
        private static float _textureOffset = 0.001f;
        private static float _tileSizeX;
        private static float _tileSizeY;
        private static Dictionary<VoxelType, TextureData> _voxelTextureData = new Dictionary<VoxelType, TextureData>();
        private VoxelDataSO _textureData;

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