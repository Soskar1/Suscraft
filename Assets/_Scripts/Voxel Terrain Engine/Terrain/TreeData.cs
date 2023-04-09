using System.Collections.Generic;
using UnityEngine;

namespace Suscraft.Core.VoxelTerrainEngine.Terrain
{
    public class TreeData
    {
        public List<Vector2Int> treePositions = new List<Vector2Int>();
        public List<Vector3Int> treeLeafesSolid = new List<Vector3Int>();
        public List<Vector3Int> treeLeafesTransparent = new List<Vector3Int>();
    }
}