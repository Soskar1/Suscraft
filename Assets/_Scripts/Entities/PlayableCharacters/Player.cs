using UnityEngine;
using UnityEngine.InputSystem;

namespace Suscraft.Core.Entities.PlayableCharacters
{
    [RequireComponent(typeof(PlayerInput))]
    [RequireComponent(typeof(Jumping))]
    [RequireComponent(typeof(GroundCheck))]
    [RequireComponent(typeof(Digging))]
    public class Player : MonoBehaviour
    {
        [SerializeField] private PlayerInput _input;
        [SerializeField] private FirstPersonCamera _firstPersonCamera;
        [SerializeField] private Jumping _jumping;
        [SerializeField] private GroundCheck _groundCheck;
        [SerializeField] private Digging _digging;
        private IMovement _movement;

        [SerializeField] private Transform _eyes;
        public Transform Eyes => _eyes;

        public void Initialize() {
            _input.Initialize();
            _input.Enable();
            _movement = GetComponent<IMovement>();

            _input.Controls.Player.Jump.performed += Jump;
            _input.Controls.Player.Dig.performed += Dig;
        }

        private void OnDisable()
        {
            _input.Controls.Player.Jump.performed -= Jump;
            _input.Controls.Player.Dig.performed -= Dig;
        }
        
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

        private void Dig(InputAction.CallbackContext context) => _digging.Dig();
    }
}