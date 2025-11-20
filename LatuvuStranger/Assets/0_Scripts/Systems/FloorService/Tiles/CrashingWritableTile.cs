using UnityEngine;
using UnityEngine.Tilemaps;
namespace Latuvu
{
    [CreateAssetMenu(menuName = "Latuvu/Tiles/Crashing Writable Tile")]
    public class CrashingWritableTile : WritableTile
    {
        public override void OnPickup(Tilemap tilemap, Vector3Int pos)
        {
            Application.Quit();
        }
    }
}
