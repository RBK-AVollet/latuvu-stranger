using System.Linq;
using UnityEngine;

namespace Latuvu
{
    public class VoidRodHUDIcon : MonoBehaviour
    {
        [System.Serializable]
        private class RodIconPerTile
        {
            public GameTile Tile;
            public Sprite Icon;
        }
        
        [SerializeField] private RodIconPerTile[] _spritePerTiles;
        [SerializeField] private Sprite _rodUnknownIcon;
        [SerializeField] private Sprite _rodNoTile;
        [SerializeField] private SpriteRenderer _renderer;

        public void UpdateIcon(GameTile tile)
        {
            if (tile == null)
            {
                _renderer.sprite = _rodNoTile;
            }
            else
            {
                var sprite = _spritePerTiles.FirstOrDefault(t => t.Tile == tile);
                _renderer.sprite = sprite != null ? sprite.Icon : _rodUnknownIcon;
            }
        }
    }
}
