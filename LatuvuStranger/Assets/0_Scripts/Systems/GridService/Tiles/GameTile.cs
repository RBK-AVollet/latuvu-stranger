using UnityEngine;

namespace Latuvu
{
    [CreateAssetMenu(menuName = "Latuvu/Tiles/Game Tile")]
    public class GameTile : UnityEngine.Tilemaps.Tile
    {
        public bool IsWalkable = true;
        public bool IsPickable = true;
    }

}
