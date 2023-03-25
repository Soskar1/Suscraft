using UnityEngine;
using Suscraft.Core.Entities;

namespace Suscraft.Core
{
    public class GameScene : MonoBehaviour
    {
        [SerializeField] private Player _player;

        private void Start()
        {
            _player.Initialize();

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}