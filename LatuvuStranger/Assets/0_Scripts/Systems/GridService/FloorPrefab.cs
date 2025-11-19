using UnityEngine;
using UnityEngine.Tilemaps;
namespace Latuvu
{
    public class FloorPrefab : MonoBehaviour
    {
        public string FloorId = "B00";
        public Tilemap Tilemap;
        public Transform EntitiesParent;
        [SerializeField] private Vector3Int _playerSpawn;

        public Vector3 GetPlayerSpawnWorld() => Tilemap.GetCellCenterWorld(_playerSpawn);
        
        private void OnDrawGizmos()
        {
            Gizmos.DrawIcon(GetPlayerSpawnWorld(), "T_PlayerSpawn.png");
        }
    }
}
