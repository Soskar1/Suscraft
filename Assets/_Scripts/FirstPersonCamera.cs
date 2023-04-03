using UnityEngine;

namespace Suscraft.Core
{
    public class FirstPersonCamera : MonoBehaviour
    {
        [SerializeField] Transform _body;

        [SerializeField][Range(0.1f, 20)] private float _sensitivity;

        private const float MAX_VIEW_ANGLE = 90f;
        [SerializeField][Range(0, MAX_VIEW_ANGLE)] private float _minViewAngle = 90f;

        private float _xRotation = 0f;

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        public virtual void Rotate(Vector2 cursorDelta)
        {
            float mouseX = cursorDelta.x * _sensitivity * Time.deltaTime;
            float mouseY = cursorDelta.y * _sensitivity * Time.deltaTime;

            _xRotation -= mouseY;
            _xRotation = Mathf.Clamp(_xRotation, -_minViewAngle, MAX_VIEW_ANGLE);

            _body.Rotate(Vector3.up * mouseX);
            transform.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);
        }
    }
}