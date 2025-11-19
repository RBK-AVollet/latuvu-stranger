using UnityEngine;

namespace Latuvu
{
    namespace ProjectNamingConventions
    {
        public class NamingConventions
        {
            // Classes & Structs: PascalCase
            public class PlayerController { }
            public struct PlayerStats { }
    
            // Methods: PascalCase
            public void MovePlayer() { }
    
            // Private fields: _camelCase
            // Protected fields: _camelCase
            private int _health;
            private float _speed;
    
            // Public fields: PascalCase
            public float MoveSpeed;
    
            // Serialized private fields: [SerializeField] 
            [UnityEngine.SerializeField] private float _jumpForce;
    
            // Properties: PascalCase
            public int Health { get; private set; }
    
            // Constants: ALL_CAPS_WITH_UNDERSCORES
            private const float MAX_SPEED = 10f;
        }
    }
}
