using UnityEngine;
using UnityEngine.Tilemaps;
namespace Latuvu
{
    [CreateAssetMenu(menuName = "Latuvu/Tiles/Staircase Tile")]
    public class StaircaseTile : GameTile
    {
        public override void OnEnter(Tilemap tilemap, Vector3Int pos)
        {
            Debug.Log("Implement go to next floor !");
        }
    }
}
