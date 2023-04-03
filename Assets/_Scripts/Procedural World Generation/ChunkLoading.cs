using Suscraft.Core.Entities;
using Suscraft.Core.VoxelTerrainEngine;
using System.Collections;
using UnityEngine;

namespace Suscraft.Core.ProceduralWorldGeneration
{
    public class ChunkLoading : MonoBehaviour
    {
        [SerializeField] private World _world;
        [SerializeField] private float _detectionTime = 1;
        private Vector3Int _currentPlayerChunkPosition;
        private Vector3Int _currentChunkCenter = Vector3Int.zero;
        private Player _player;

        private void Awake() => _world.OnNewChunksGenerated += StartCheckingMap;
        private void OnDisable() => _world.OnNewChunksGenerated -= StartCheckingMap;

        public void Initialize(Player player)
        {
            _player = player;
            StartCheckingMap();
        }

        private void StartCheckingMap()
        {
            SetCurrentChunkCoordinates();
            StopAllCoroutines();
            StartCoroutine(CheckChunkLoadingRequest());
        }

        private IEnumerator CheckChunkLoadingRequest()
        {
            yield return new WaitForSeconds(_detectionTime);
            if (Mathf.Abs(_currentChunkCenter.x - _player.transform.position.x) > _world.ChunkSize ||
                Mathf.Abs(_currentChunkCenter.z - _player.transform.position.z) > _world.ChunkSize ||
                Mathf.Abs(_currentPlayerChunkPosition.y - _player.transform.position.y) > _world.ChunkHeight)
            {
                _world.LoadAdditionalChunksRequest(_player.transform);
            } else
            {
                StartCoroutine(CheckChunkLoadingRequest());
            }
        }

        private void SetCurrentChunkCoordinates()
        {
            _currentPlayerChunkPosition = WorldDataHelper.ChunkPositionFromVoxelCoords(_world, Vector3Int.RoundToInt(_player.transform.position));
            _currentChunkCenter.x = _currentPlayerChunkPosition.x + _world.ChunkSize / 2;
            _currentChunkCenter.z = _currentPlayerChunkPosition.z + _world.ChunkSize / 2;
        }
    }
}