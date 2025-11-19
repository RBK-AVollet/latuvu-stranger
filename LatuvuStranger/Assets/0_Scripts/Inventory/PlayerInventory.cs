using UnityEngine;

namespace Latuvu
{
    public class PlayerInventory
    {
        // Key Items
        public bool HasRod { get; private set; }
        public bool HasCube { get; private set; }
        public bool HasSword { get; private set; }
        
        public void ObtainRod() { HasRod = true; }
        public void ObtainCube() { HasCube = true; }
        public void ObtainSword() { HasSword = true; }
        
        // Collectibles
        public int Crickets { get; private set; }
        
        public void ObtainCrickets(int amount) { Crickets += amount; }
        public void RemoveCrickets(int amount, bool canNegative)
        {
            Crickets -= amount;
            if (!canNegative && Crickets < 0) Crickets = 0;
        }
    }
}
