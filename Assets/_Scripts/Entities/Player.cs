using UnityEngine;

namespace Suscraft.Core.Entities
{
    [RequireComponent(typeof(PlayerInput))]
    public class Player : MonoBehaviour
    {
        [SerializeField] private PlayerInput _input;
        [SerializeField] private FirstPersonCamera _firstPersonCamera;
        [SerializeField] private IMovement _movement;

        public void Initialize() {
            _input.Initialize();
            _input.Enable();
            _movement = GetComponent<IMovement>();
        }

        private void Update()
        {
            _firstPersonCamera.Rotate(_input.GetDeltaMouse());
        }

        private void FixedUpdate()
        {
            if (_movement != null)
                _movement.Move(_input.GetMovementDirection());
        }
    }
}