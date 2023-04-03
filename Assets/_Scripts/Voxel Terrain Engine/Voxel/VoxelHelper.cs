using UnityEngine;
using Suscraft.Core.VoxelTerrainEngine.Chunks;

namespace Suscraft.Core.VoxelTerrainEngine.Voxels
{
    public static class VoxelHelper
    {
        private static Direction[] _directions =
        {
            Direction.back,
            Direction.down,
            Direction.forward,
            Direction.left,
            Direction.right,
            Direction.up
        };

        public static MeshData GetMeshData(ChunkData chunkData, int x, int y, int z, MeshData meshData, VoxelType voxelType)
        {
            if (voxelType == VoxelType.Air || voxelType == VoxelType.Nothing)
                return meshData;

            foreach (var direction in _directions)
            {
                var neighbourVortexCoordinates = new Vector3Int(x, y, z) + direction.GetVector();
                var neighbourVoxelType = Chunk.GetVoxelFromChunkCoordinates(chunkData, neighbourVortexCoordinates);

                if (neighbourVoxelType != VoxelType.Nothing && VoxelDataManager.VoxelTextureData[neighbourVoxelType].isSolid == false)
                {
                    if (voxelType == VoxelType.Water)
                    {
                        if (neighbourVoxelType == VoxelType.Air)
                            meshData.waterMesh = GetFaceDataIn(direction, chunkData, x, y, z, meshData.waterMesh, voxelType);
                    } 
                    else
                    {
                        meshData = GetFaceDataIn(direction, chunkData, x, y, z, meshData, voxelType);
                    }
                }
            }

            return meshData;
        }

        public static MeshData GetFaceDataIn(Direction direction, ChunkData chunkData, int x, int y, int z, MeshData meshData, VoxelType voxelType)
        {
            GetFaceVertices(direction, x, y, z, meshData, voxelType);
            meshData.AddQuadTriangles(VoxelDataManager.VoxelTextureData[voxelType].generatesCollider);
            meshData.UV.AddRange(FaceUVs(direction, voxelType));

            return meshData;
        }

        public static void GetFaceVertices(Direction direction, int x, int y, int z, MeshData meshData, VoxelType voxelType)
        {
            var generatesCollider = VoxelDataManager.VoxelTextureData[voxelType].generatesCollider;

            switch (direction)
            {
                case Direction.back:
                    meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z - 0.5f), generatesCollider);
                    meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z - 0.5f), generatesCollider);
                    meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z - 0.5f), generatesCollider);
                    meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z - 0.5f), generatesCollider);
                    break;
                case Direction.forward:
                    meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z + 0.5f), generatesCollider);
                    meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z + 0.5f), generatesCollider);
                    meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z + 0.5f), generatesCollider);
                    meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z + 0.5f), generatesCollider);
                    break;
                case Direction.left:
                    meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z + 0.5f), generatesCollider);
                    meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z + 0.5f), generatesCollider);
                    meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z - 0.5f), generatesCollider);
                    meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z - 0.5f), generatesCollider);
                    break;
                case Direction.right:
                    meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z - 0.5f), generatesCollider);
                    meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z - 0.5f), generatesCollider);
                    meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z + 0.5f), generatesCollider);
                    meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z + 0.5f), generatesCollider);
                    break;
                case Direction.down:
                    meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z - 0.5f), generatesCollider);
                    meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z - 0.5f), generatesCollider);
                    meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z + 0.5f), generatesCollider);
                    meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z + 0.5f), generatesCollider);
                    break;
                case Direction.up:
                    meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z + 0.5f), generatesCollider);
                    meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z + 0.5f), generatesCollider);
                    meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z - 0.5f), generatesCollider);
                    meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z - 0.5f), generatesCollider);
                    break;
                default:
                    break;
            }
        }

        public static Vector2[] FaceUVs(Direction direction, VoxelType voxelType)
        {
            Vector2[] UVs = new Vector2[4];
            var tilePos = TexturePosition(direction, voxelType);

            UVs[0] = new Vector2(VoxelDataManager.TileSizeX * tilePos.x + VoxelDataManager.TileSizeX - VoxelDataManager.TextureOffset,
                VoxelDataManager.TileSizeY * tilePos.y + VoxelDataManager.TextureOffset);

            UVs[1] = new Vector2(VoxelDataManager.TileSizeX * tilePos.x + VoxelDataManager.TileSizeX - VoxelDataManager.TextureOffset,
                VoxelDataManager.TileSizeY * tilePos.y + VoxelDataManager.TileSizeY - VoxelDataManager.TextureOffset);

            UVs[2] = new Vector2(VoxelDataManager.TileSizeX * tilePos.x + VoxelDataManager.TextureOffset,
                VoxelDataManager.TileSizeY * tilePos.y + VoxelDataManager.TileSizeY - VoxelDataManager.TextureOffset);

            UVs[3] = new Vector2(VoxelDataManager.TileSizeX * tilePos.x + VoxelDataManager.TextureOffset,
                VoxelDataManager.TileSizeY * tilePos.y + VoxelDataManager.TextureOffset);

            return UVs;
        }

        public static Vector2Int TexturePosition(Direction direction, VoxelType voxelType)
        {
            return direction switch
            {
                Direction.up => VoxelDataManager.VoxelTextureData[voxelType].up,
                Direction.down => VoxelDataManager.VoxelTextureData[voxelType].down,
                _ => VoxelDataManager.VoxelTextureData[voxelType].side
            };
        }
    }
}