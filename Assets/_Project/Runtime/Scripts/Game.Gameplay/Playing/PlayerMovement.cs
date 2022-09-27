using Unity.Netcode;
using UnityEngine;

namespace Game.Gameplay.Playing
{
    public sealed class PlayerMovement : NetworkBehaviour
    {
        [SerializeField] private Rigidbody2D _rigidBody;
        [SerializeField] private float _moveSpeed;
        
        public void Begin()
        { }
        
        public void Stop()
        {}
        
        public void Tick(float deltaTime)
        {
            float horizontalInput = UnityEngine.Input.GetAxis("Horizontal");
            float verticalInput = UnityEngine.Input.GetAxis("Vertical");
            
            Vector2 movementInput = new Vector2(horizontalInput, verticalInput).normalized;

            _rigidBody.velocity = movementInput * _moveSpeed;
        }
    }
}