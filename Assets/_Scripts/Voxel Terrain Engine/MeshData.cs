using UnityEngine;
using System.Collections.Generic;

namespace Suscraft.Core.VoxelTerrainEngine
{
    public class MeshData
    {
        private List<Vector3> _vertices = new List<Vector3>();
        private List<int> _triangles = new List<int>();
        private List<Vector2> _uv = new List<Vector2>();

        private List<Vector3> _colliderVertices = new List<Vector3>();
        private List<int> _colliderTriangles = new List<int>();

        private MeshData _waterMesh;

        private bool _isMainMesh = true;

        public List<Vector3> Vertices => _vertices;
        public List<int> Triangles => _triangles;
        public List<Vector2> UV => _uv;
        public List<Vector3> ColliderVertices => _colliderVertices;
        public List<int> ColliderTriangles => _colliderTriangles;
        public MeshData WaterMesh => _waterMesh;

        public MeshData(bool isMainMesh)
        {
            if (isMainMesh)
                _waterMesh = new MeshData(false);
        }

        public void AddVertex(Vector3 vertex, bool vertexGeneratesCollider)
        {
            _vertices.Add(vertex);
            if (vertexGeneratesCollider)
                _colliderVertices.Add(vertex);
        }

        public void AddQuadTriangles(bool quadGeneratesCollider)
        {
            _triangles.Add(_vertices.Count - 4);
            _triangles.Add(_vertices.Count - 3);
            _triangles.Add(_vertices.Count - 2);

            _triangles.Add(_vertices.Count - 4);
            _triangles.Add(_vertices.Count - 2);
            _triangles.Add(_vertices.Count - 1);

            if (quadGeneratesCollider)
            {
                _colliderTriangles.Add(_colliderVertices.Count - 4);
                _colliderTriangles.Add(_colliderVertices.Count - 3);
                _colliderTriangles.Add(_colliderVertices.Count - 2);
                _colliderTriangles.Add(_colliderVertices.Count - 4);
                _colliderTriangles.Add(_colliderVertices.Count - 2);
                _colliderTriangles.Add(_colliderVertices.Count - 1);
            }
        }
    }
}