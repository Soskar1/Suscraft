using Cinemachine;
using Suscraft.Core.Entities.PlayableCharacters;
using Suscraft.Core.ProceduralWorldGeneration;
using Suscraft.Core.VoxelTerrainEngine;
using UnityEngine;

namespace Suscraft.Core
{
    public class GameScene : MonoBehaviour
    {
        [SerializeField] private Player _playerPrefab;
        [SerializeField] private World _world;
        [SerializeField] private CinemachineVirtualCamera _virtualCamera;

        [SerializeField] private ChunkLoading _chunkLoading;

        private void Awake() => _world.OnWorldGenerated += SpawnPlayer;
        private void OnDisable() => _world.OnWorldGenerated -= SpawnPlayer;

        public void SpawnPlayer()
        {
            Vector3 raycastStartPoint = new Vector3Int(_world.ChunkSize / 2, _world.ChunkHeight, _world.ChunkSize / 2);
            RaycastHit hit;
            if (Physics.Raycast(raycastStartPoint, Vector3.down, out hit, _world.ChunkHeight))
            {
                Player playerInstance = Instantiate(_playerPrefab, hit.point + Vector3Int.up, Quaternion.identity);
                _virtualCamera.Follow = playerInstance.Eyes;
                _chunkLoading.Initialize(playerInstance);
                playerInstance.Initialize();
            }

            _world.OnWorldGenerated -= SpawnPlayer;
        }
    }
}