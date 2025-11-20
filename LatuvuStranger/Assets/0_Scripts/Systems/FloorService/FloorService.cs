using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Latuvu
{
    public class FloorService : Singleton<FloorService>
    {
        [SerializeField] private FloorPrefab[] _floors;
        [SerializeField] private TileBase[] _hud;
        
        private List<TileEntity> _entities = new ();
        
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
            SpawnEntities();
        }
        
        private void ApplyHUD()
        {
            for (int i = 0; i < _hud.Length; i++)
            {
                Tilemap.SetTile(new Vector3Int(i, 0, 0), _hud[i]);
            }
        }
        
        private void SpawnEntities()
        {
            _entities.Clear();
            
            foreach (var entity in _currentFloor.Entities)
            {
                var pos = Tilemap.GetCellCenterWorld(entity.Position);
                _entities.Add(Instantiate(entity.EntityPrefab, pos, Quaternion.identity, transform));
            }
        }
        
        public bool TryGetEntityAtPos(Vector3Int pos, out TileEntity entity)
        {
            entity = _entities.FirstOrDefault(e => e.Position == (Vector2Int)pos);
            return entity != null;
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
