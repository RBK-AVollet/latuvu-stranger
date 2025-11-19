using UnityEngine;

namespace Latuvu
{
    public class TileEntity : MonoBehaviour
    {
        [field:SerializeField] public Vector2Int Position { get; private set; }
        [field:SerializeField] public Sprite Skin { get; private set; }
    }
}
