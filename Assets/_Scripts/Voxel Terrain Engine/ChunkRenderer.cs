using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Suscraft.Core.VoxelTerrainEngine
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
    public class ChunkRenderer : MonoBehaviour
    {
        [SerializeField] private MeshFilter _meshFilter;
        [SerializeField] private MeshCollider _meshCollider;
        private Mesh _mesh;

        [SerializeField] private bool _showGizmo = false;

        public ChunkData ChunkData { get; private set; }

        public bool ModifiedByPlayer
        {
            get { return ChunkData.modifiedByPlayer; }
            set { ChunkData.modifiedByPlayer = value; }
        }

        private void Awake() => _mesh = _meshFilter.mesh;

        public void InitializeChunk(ChunkData data) => ChunkData = data;

        private void RenderMesh(MeshData meshData)
        {
            _mesh.Clear();

            _mesh.subMeshCount = 2;
            _mesh.vertices = meshData.Vertices.Concat(meshData.waterMesh.Vertices).ToArray();

            _mesh.SetTriangles(meshData.Triangles.ToArray(), 0);
            _mesh.SetTriangles(meshData.waterMesh.Triangles.Select(val => val + meshData.Vertices.Count).ToArray(), 1);

            _mesh.uv = meshData.UV.Concat(meshData.waterMesh.UV).ToArray();
            _mesh.RecalculateNormals();

            _meshCollider.sharedMesh = null;
            Mesh collisionMesh = new Mesh();
            collisionMesh.vertices = meshData.ColliderVertices.ToArray();
            collisionMesh.triangles = meshData.ColliderTriangles.ToArray();
            collisionMesh.RecalculateNormals();

            _meshCollider.sharedMesh = collisionMesh;
        }

        public void UpdateChunk() => RenderMesh(Chunk.GetChunkMeshData(ChunkData));

        public void UpdateChunk(MeshData data) => RenderMesh(data);

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (_showGizmo)
            {
                if (Application.isPlaying && ChunkData != null)
                {
                    if (Selection.activeObject == gameObject)
                        Gizmos.color = new Color(0, 1, 0, 0.4f);
                    else
                        Gizmos.color = new Color(1, 0, 1, 0.4f);

                    Gizmos.DrawCube(transform.position + new Vector3(ChunkData.ChunkSize / 2f, ChunkData.ChunkHeight / 2f,
                        ChunkData.ChunkSize / 2f), new Vector3(ChunkData.ChunkSize, ChunkData.ChunkHeight, ChunkData.ChunkSize));
                }
            }
        }
#endif
    }
}