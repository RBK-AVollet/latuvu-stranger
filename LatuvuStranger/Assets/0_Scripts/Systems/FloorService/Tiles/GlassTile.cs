using UnityEngine;
using UnityEngine.Tilemaps;
namespace Latuvu
{
    [CreateAssetMenu(menuName = "Latuvu/Tiles/Glass Tile")]
    public class GlassTile : GameTile
    {
        public BrokenGlassTile BrokenGlassTile;
        
        public override void OnEnter(Tilemap tilemap, Vector3Int pos)
        {
            tilemap.SetTile(pos, BrokenGlassTile);
        }
    }
}
