using Padrox.Acelab.Modules.Audio;
using UnityEngine;
using UnityEngine.Tilemaps;
namespace Latuvu
{
    [CreateAssetMenu(menuName = "Latuvu/Tiles/Staircase Tile")]
    public class StaircaseTile : GameTile
    {
        [SerializeField] private SoundData _sfx;
        
        public override void OnEnter(Tilemap tilemap, Vector3Int pos)
        {
            FloorService.Instance.LoadNextFloor();
            SoundController.Instance.CreateSound()
                .WithSoundData(_sfx)
                .Play();
        }
    }
}
