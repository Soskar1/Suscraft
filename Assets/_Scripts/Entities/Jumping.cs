using UnityEngine;

namespace Suscraft.Core.Entities
{
    public class Jumping : MonoBehaviour
    {
        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private float _force;

        public void Jump() => _rigidbody.AddForce(Vector2.up * _force, ForceMode.Impulse);
    }
}