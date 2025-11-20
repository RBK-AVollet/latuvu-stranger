using UnityEngine;
using UnityEngine.Tilemaps;
namespace Latuvu
{
    [CreateAssetMenu(menuName = "Latuvu/Tiles/Glass Tile")]
    public class GlassTile : GameTile
    {
        public GameObject BreakingGlassPrefab;
        
        public override void OnExit(Tilemap tilemap, Vector3Int pos)
        {
            tilemap.SetTile(pos, null);
            Instantiate(BreakingGlassPrefab, tilemap.GetCellCenterWorld(pos), Quaternion.identity);
        }
    }
}
