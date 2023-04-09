using System.Collections.Generic;
using UnityEngine;

namespace Suscraft.Core.VoxelTerrainEngine.Terrain
{
    public class TreeData
    {
        public List<Vector2Int> treePositions = new List<Vector2Int>();
        private List<Vector3Int> _treeLeafesSolid = new List<Vector3Int>();
        private List<Vector3Int> _treeLeafesTransparent = new List<Vector3Int>();
    }
}