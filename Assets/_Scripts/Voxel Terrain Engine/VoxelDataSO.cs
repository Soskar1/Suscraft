using System;
using System.Collections.Generic;
using UnityEngine;

namespace Suscraft.Core.VoxelTerrainEngine
{
    [CreateAssetMenu(fileName = "Voxel Data", menuName = "Data/Voxel Data")]
    public class VoxelDataSO : ScriptableObject
    {
        public float textureSizeX;
        public float textureSizeY;
        public List<TextureData> textureDataList;
    }

    [Serializable]
    public struct TextureData
    {
        public VoxelType voxelType;

        public Vector2Int up;
        public Vector2Int down;
        public Vector2Int side;

        public bool isSolid;
        public bool generatesCollider;
    }
}