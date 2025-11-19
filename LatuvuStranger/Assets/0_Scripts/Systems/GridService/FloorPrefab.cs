using UnityEngine;
using UnityEngine.Tilemaps;
namespace Latuvu
{
    public class FloorPrefab : MonoBehaviour
    {
        public string FloorId = "B00";
        public Tilemap Tilemap;
        public Transform EntitiesParent;
        public Transform PlayerSpawn;
    }
}
