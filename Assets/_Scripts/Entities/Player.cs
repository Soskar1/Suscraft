using UnityEngine;
using UnityEngine.InputSystem;

namespace Suscraft.Core.Entities
{
    [RequireComponent(typeof(PlayerInput), typeof (Jumping), typeof(GroundCheck))]
    public class Player : MonoBehaviour
    {
        [SerializeField] private PlayerInput _input;
        [SerializeField] private FirstPersonCamera _firstPersonCamera;
        [SerializeField] private Jumping _jumping;
        [SerializeField] private GroundCheck _groundCheck;
        private IMovement _movement;

        private void Start()
        {
            Initialize();
        }

        [ContextMenu("Initialize")]
        public void Initialize() {
            _input.Initialize();
            _input.Enable();
            _movement = GetComponent<IMovement>();

            _input.Controls.Player.Jump.performed += Jump;
        }

        private void OnDisable() => _input.Controls.Player.Jump.performed -= Jump;

        private void Update() => _firstPersonCamera.Rotate(_input.GetDeltaMouse());

        private void FixedUpdate()
        {
            if (_movement != null)
                _movement.Move(_input.GetMovementDirection());
        }

        private void Jump(InputAction.CallbackContext context)
        {
            if (_groundCheck.CheckForGround())
                _jumping.Jump();
        }
    }
}