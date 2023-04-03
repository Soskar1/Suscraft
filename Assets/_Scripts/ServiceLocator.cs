using Suscraft.Core.VoxelTerrainEngine;
using UnityEngine;

namespace Suscraft.Core
{
    public class ServiceLocator : MonoBehaviour
    {
        [SerializeField] private World _world;

        public static ServiceLocator Instance { get; private set; }
        public World World { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
                return;
            }

            Instance = this;
            World = _world;
        }
    }
}