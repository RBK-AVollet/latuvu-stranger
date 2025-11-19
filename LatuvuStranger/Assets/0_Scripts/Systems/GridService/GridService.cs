using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Latuvu
{
    public class FloorService : Singleton<FloorService>
    {
        [SerializeField] private FloorPrefab[] _floors;
        private FloorPrefab _currentFloor;
        
        public Tilemap Tilemap => _currentFloor.Tilemap;
        
        public T GetTile<T>(Vector3Int position) where T : TileBase => Tilemap.GetTile<T>(position);
        
        public void LoadFloor(string floorId)
        {
            var floor = GetFloorById(floorId);
            if (floor == null)
            {
                Debug.LogWarning($"[FloorService]: No valid floor found for the following ID {floorId}.");
                return;
            }

            if (_currentFloor)
            {
                Destroy(_currentFloor);
            }

            _currentFloor = Instantiate(floor, transform);
            Debug.Log($"[FloorService]: Loaded floor {floorId}");
        }

        public FloorPrefab GetFloorById(string floorId)
        {
            return _floors.FirstOrDefault(f => f.FloorId == floorId);
        }

        protected override void Awake()
        {
            base.Awake();
            LoadFloor("B00");
        }
    }
}
