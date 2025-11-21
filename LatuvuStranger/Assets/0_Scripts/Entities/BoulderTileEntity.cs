using Padrox.Acelab.Modules.Audio;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Latuvu
{
    public class BoulderTileEntity : StaticTileEntity
    {
        [SerializeField] protected SoundData _moveSound;
        
        public override bool TryMove(Vector3Int relativePosition, Tilemap tilemap)
        {
            if (base.TryMove(relativePosition, tilemap))
            {
                SoundController.Instance.CreateSound()
                    .WithSoundData(_moveSound)
                    .Play();
                return true;
            }

            return false;
        }
    }
}
