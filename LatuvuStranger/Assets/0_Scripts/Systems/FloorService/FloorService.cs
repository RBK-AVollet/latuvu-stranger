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
        [SerializeField] private Transform _entitiesContainer;
        [SerializeField] private string _defaultFloor = "B000";
        
        private List<TileEntity> _entities = new ();

        private FloorPrefab _currentFloor;

        private const string k_floorSaveKey = "FloorId";
        
        public Tilemap Tilemap => _currentFloor.Tilemap;
        
        public FloorPrefab CurrentFloor => _currentFloor;
        
        public PlayerTileEntity Player { get; private set; }
        
        public bool TryGetEntityAtPos(Vector3Int pos, out TileEntity entity)
        {
            entity = _entities.FirstOrDefault(e => e.Position == pos);
            return entity != null;
        }
        
        public bool TryGetLivingEntityAtPos(Vector3Int pos, out LivingTileEntity livingEntity)
        {
            livingEntity = null;
            TileEntity e = _entities.FirstOrDefault(e => e.Position == pos);
            
            if (e is not LivingTileEntity entity) return false;
            
            livingEntity = entity;
            return true;
        }
        
        public bool TryGetStaticEntityAtPos(Vector3Int pos, out StaticTileEntity staticEntity)
        {
            staticEntity = null;
            TileEntity e = _entities.FirstOrDefault(e => e.Position == pos);
            
            if (e is not StaticTileEntity entity) return false;
            
            staticEntity = entity;
            return true;
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

        public void LoadNextFloor(int incrementAmount)
        {
            int currId = int.Parse(_currentFloor.FloorId.Substring(2, 2));
            int nextId = currId + incrementAmount;
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
            WriteOnCell(new Vector3Int(12, 0, 0), _currentFloor.FloorId.Substring(0, 2));
            WriteOnCell(new Vector3Int(13, 0, 0), _currentFloor.FloorId.Substring(2, 2));
            UpdateCrickets(Player.Inventory.Crickets);
            
            SpawnEntities();
            
            Player.Respawn();
            
            #if !UNITY_EDITOR
            ES3.Save(k_floorSaveKey, floorId);
            Debug.Log($"Saved floor {floorId} !");
            #endif
        }
        
        public void UpdateCrickets(int cricketCount)
        {
            string cricketText = (cricketCount < 10 ? "0" : "") + cricketCount;
            WriteOnCell(new Vector3Int(5, 0, 0), cricketText);
        }

        public void UpdateVoidRodTile(GameTile tile)
        {
            var go = Tilemap.GetInstantiatedObject(new Vector3Int(6, 0, 0));
            if (!go) return;

            var comp = go.GetComponent<VoidRodHUDIcon>();
            if (!comp) return;
            
            comp.UpdateIcon(tile);
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

            foreach (Transform child in _entitiesContainer)
            {
                Destroy(child.gameObject);
            }
            
            foreach (var entity in _currentFloor.Entities)
            {
                var pos = Tilemap.GetCellCenterWorld(entity.Position);
                TileEntity e = Instantiate(entity.EntityPrefab, pos, Quaternion.identity, _entitiesContainer);
                e.Position = entity.Position;
                e.Direction = entity.Direction;
                _entities.Add(e);
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
            Player = playerGo.GetComponent<PlayerTileEntity>();

            #if !UNITY_EDITOR
            string floorId = ES3.Load(k_floorSaveKey, defaultValue: _defaultFloor);
            LoadFloor(floorId);
            #else
            LoadFloor(_defaultFloor);
            #endif
        }

        public void RemoveEntity(TileEntity entity)
        {
            if (entity == null) return;
            _entities.Remove(entity);
        }
    }
}
