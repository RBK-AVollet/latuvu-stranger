using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Latuvu
{
    public class FloorService : Singleton<FloorService>
    {
        [SerializeField] private FloorPrefab[] _floors;
        [SerializeField] private TileBase[] _hud;
        private FloorPrefab _currentFloor;
        
        public Tilemap Tilemap => _currentFloor.Tilemap;
        
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
            ApplyHUD();
        }
        
        private void ApplyHUD()
        {
            for (int i = 0; i < _hud.Length; i++)
            {
                Tilemap.SetTile(new Vector3Int(i, 0, 0), _hud[i]);
            }
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
