using UnityEngine;

namespace Suscraft.Core
{
    public class PlayerInput : MonoBehaviour
    {
        private Controls _controls;
        public Controls Controls => _controls;

        public void Initialize() => _controls = new Controls();
        public void Enable() => _controls.Enable();
        public void Disable() => _controls.Disable();

        public Vector2 GetMovementDirection() => _controls.Player.Movement.ReadValue<Vector2>();
    }
}