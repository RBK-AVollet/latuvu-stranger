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
        
        public virtual void TryInteract(PlayerTileEntity player)
        {
            Debug.Log("Interacted with tile entity at position: " + Position);
        }

        protected virtual void Tick()
        { }
    }
}
