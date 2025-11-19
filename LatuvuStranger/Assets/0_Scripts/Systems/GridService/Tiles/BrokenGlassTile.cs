using UnityEngine;
using UnityEngine.Tilemaps;
namespace Latuvu
{
    [CreateAssetMenu(menuName = "Latuvu/Tiles/Broken Glass Tile")]
    public class BrokenGlassTile : GameTile
    {
        public override void OnExit(Tilemap tilemap, Vector3Int pos)
        {
            tilemap.SetTile(pos, null);
        }
    }
}
