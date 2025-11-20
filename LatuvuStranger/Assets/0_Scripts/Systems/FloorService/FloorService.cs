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
        [SerializeField] private GameObject _playerPrefab;
        
        private List<TileEntity> _entities = new ();
        
        private FloorPrefab _currentFloor;
        
        public Tilemap Tilemap => _currentFloor.Tilemap;
        
        public FloorPrefab CurrentFloor => _currentFloor;
        
        public PlayerController Player { get; private set; }
        
        public bool TryGetEntityAtPos(Vector3Int pos, out TileEntity entity)
        {
            entity = _entities.FirstOrDefault(e => e.Position == (Vector2Int)pos);
            return entity != null;
        }

        public void LoadNextFloor()
        {
            int currId = int.Parse(_currentFloor.FloorId.Substring(2, 2));
            int nextId = currId + 1;
            string floorId = "B0" + (nextId < 10 ? "0" : "") + nextId;

            if (GetFloorById(floorId) == null)
            {
                Debug.Log("Reached final level ! Well done, cannot go further down !");
                return;
            }
            
            LoadFloor(floorId);
        }
        
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
                Destroy(_currentFloor.gameObject);
            }

            _currentFloor = Instantiate(floor, transform);
            
            ApplyHUD();
            WriteOnCell(new Vector3Int(1, 0, 0), "VO");
            WriteOnCell(new Vector3Int(2, 0, 0), "ID");
            WriteOnCell(new Vector3Int(5, 0, 0), "00");
            WriteOnCell(new Vector3Int(12, 0, 0), _currentFloor.FloorId.Substring(0, 2));
            WriteOnCell(new Vector3Int(13, 0, 0), _currentFloor.FloorId.Substring(2, 2));
            
            SpawnEntities();
            
            Player.Respawn();
        }

        public FloorPrefab GetFloorById(string floorId)
        {
            return _floors.FirstOrDefault(f => f.FloorId == floorId);
        }
        
        private void ApplyHUD()
        {
            for (int i = 0; i < _hud.Length; i++)
            {
                var position = new Vector3Int(i, 0, 0);
                Tilemap.SetTile(position, _hud[i]);
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
        
        private void WriteOnCell(Vector3Int pos, string content)
        {
            var go = Tilemap.GetInstantiatedObject(pos);
            if (!go) return;

            var comp = go.GetComponent<WritableTileText>();
            if (!comp) return;
            
            comp.Write(content);
        }

        protected override void Awake()
        {
            base.Awake();
            
            var playerGo = Instantiate(_playerPrefab);
            Player = playerGo.GetComponent<PlayerController>();
            
            LoadFloor("B001");
        }
    }
}
