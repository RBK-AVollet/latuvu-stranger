using UnityEngine;

namespace Latuvu
{
    public class PlayerInventory
    {
        private FloorService _floorService;
        
        const string k_cricketSaveKey = "CricketCount";
        
        // Key Items
        public bool HasWand { get; private set; }
        public bool HasCube { get; private set; }
        public bool HasSword { get; private set; }

        public GameTile Tile { get; private set; }
        public bool HasTile => Tile != null;

        public void ObtainWand()
        {
            HasWand = true;
            FloorService.Instance.Player.EnableGridMovement();
        }

        public void ObtainCube() { HasCube = true; }
        public void RemoveCube() { HasCube = false; }
        public void ObtainSword() { HasSword = true; }
        
        // Collectibles
        public int Crickets { get; private set; } = 5;
        
        public void ObtainCrickets(int amount)
        {
            Crickets += amount;
            _floorService.UpdateCrickets(Crickets);
        }

        public void RemoveCrickets(int amount, bool canNegative)
        {
            Crickets -= amount;
            if (!canNegative && Crickets < 0) Crickets = 0;

            _floorService.UpdateCrickets(Crickets);
        }

        public PlayerInventory()
        {
            HasWand = true;
            HasCube = false;
            HasSword = false;

            _floorService = FloorService.Instance;

            #if !UNITY_EDITOR
            Crickets = ES3.Load(k_cricketSaveKey, 0);

            #if !UNITY_EDITOR
            Crickets = ES3.Load(k_cricketSaveKey, 0);
            #endif
        }

        ~PlayerInventory()
        {
            #if !UNITY_EDITOR
            ES3.Save(k_cricketSaveKey, Crickets);
            #endif
        }

        ~PlayerInventory()
        {
            #if !UNITY_EDITOR
            ES3.Save(k_cricketSaveKey, Crickets);
            #endif
        }
        
        public void StoreTile(GameTile tile)
        {
            Tile = tile;
            _floorService.UpdateVoidRodTile(tile);
        }

        public void ClearTile()
        {
            Tile = null;
            _floorService.UpdateVoidRodTile(null);
        }
    }
}