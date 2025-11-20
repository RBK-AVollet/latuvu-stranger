using UnityEngine;
using UnityEngine.Tilemaps;

namespace Latuvu
{

    [CreateAssetMenu(menuName = "Latuvu/Tiles/Game Tile")]
    public class GameTile : Tile
    {
        public bool IsWalkable = true;
        public bool IsPickable = true;
        
        public virtual void OnEnter(Tilemap tilemap, Vector3Int pos)  { }
        public virtual void OnExit(Tilemap tilemap, Vector3Int pos)   { }
        public virtual void OnPickup(Tilemap tilemap, Vector3Int pos) { }
    }
}
