using UnityEngine;

namespace Latuvu
{
    public class VoidRodHUDIcon : MonoBehaviour
    {
        [SerializeField] private Sprite _voidRod;
        [SerializeField] private Sprite _voidRodHasTile;
        [SerializeField] private SpriteRenderer _renderer;

        public void UpdateIcon(bool hasTile)
        {
            _renderer.sprite = hasTile ? _voidRodHasTile : _voidRod;
        }
    }
}
