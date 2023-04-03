using Suscraft.Core.VoxelTerrainEngine;
using Suscraft.Core.VoxelTerrainEngine.Voxels;
using UnityEngine;

namespace Suscraft.Core.Entities.PlayableCharacters
{
    public class Digging : MonoBehaviour
    {
        [SerializeField] private float _diggingLength = 5f;
        [SerializeField] private LayerMask _groundLayer;
        private World _world;
        private Camera _mainCamera;

        private void Awake()
        {
            _mainCamera = Camera.main;
            _world = ServiceLocator.Instance.World;
        }

        public void Dig()
        {
            Ray ray = new Ray(_mainCamera.transform.position, _mainCamera.transform.forward);

            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, _diggingLength, _groundLayer))
                ModifyTerrain(hit);
        }

        private void ModifyTerrain(RaycastHit hit) => _world.SetVoxel(hit, VoxelType.Air);
    }
}