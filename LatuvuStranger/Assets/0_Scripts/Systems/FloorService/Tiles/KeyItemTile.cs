using UnityEngine;
using UnityEngine.Tilemaps;

namespace Latuvu
{
    [CreateAssetMenu(menuName = "Latuvu/Tiles/Key Item Tile")]
    public class KeyItemTile : GameTile
    {
        public override void OnPickup(Tilemap tilemap, Vector3Int pos)
        {
            Application.Quit();
        }
    }
}
