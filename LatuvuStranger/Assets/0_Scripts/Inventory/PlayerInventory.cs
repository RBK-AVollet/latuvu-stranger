using UnityEngine.InputSystem;

namespace Latuvu
{
    public class PlayerInventory
    {
        private FloorService _floorService;

        public bool HasWand { get; private set; }
        public bool HasCube { get; private set; }
        public bool HasSword { get; private set; }

        public GameTile Tile { get; private set; }
        public bool HasTile => Tile != null;

        public int Crickets { get; private set; } = 5;

        public PlayerInventory()
        {
            HasWand = false;
            HasCube = false;
            HasSword = false;

            _floorService = FloorService.Instance;
        }

        public void ObtainWand()
        {
            HasWand = true;
            FloorService.Instance.Player.EnableGridMovement();
        }

        public void ObtainCube() { HasCube = true; }
        public void RemoveCube() { HasCube = false; }
        public void ObtainSword() { HasSword = true; }

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