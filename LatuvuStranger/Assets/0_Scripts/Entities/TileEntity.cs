using System;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Latuvu
{
    public class TileEntity : MonoBehaviour
    {
        [field:SerializeField] public Vector3Int Position { get; set; }
        [field:SerializeField] public Vector3Int Direction { get; set; }
        [field:SerializeField] public AffectDirection MoveDirections { get; private set; }
        [field:SerializeField] public AffectDirection InteractDirections { get; private set; }

        protected virtual void Start()
        {
            GameService.Instance.RegisterTickAction(Tick);
        }

        private void OnDestroy()
        {
            if (GameService.Instance != null)
            {
                GameService.Instance.UnregisterTickAction(Tick);
            }
        }
        
        public bool TryInteract(Vector3Int relativePosition, Tilemap tilemap, PlayerTileEntity player)
        {
            
            Vector3Int moveDir = Vector3Int.zero;
            if (relativePosition.x != 0 && relativePosition.y == 0)
                moveDir = (relativePosition.x > 0) ? Vector3Int.right : Vector3Int.left;
            else if (relativePosition.y != 0 && relativePosition.x == 0)
                moveDir = (relativePosition.y > 0) ? Vector3Int.up : Vector3Int.down;
            else
                return false;

            if (!InteractDirections.AllowsMovement(moveDir)) return false;

            Interact(player);
            
            return true;
        }

        protected virtual void Interact(PlayerTileEntity player)
        {
            Debug.Log("Interacted with tile entity at position: " + Position);
        }

        protected virtual void Tick()
        { }
    }
}
