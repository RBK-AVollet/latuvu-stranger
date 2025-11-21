using Padrox.Acelab.Modules.Audio;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;
namespace Latuvu
{
    [CreateAssetMenu(menuName = "Latuvu/Tiles/Glass Tile")]
    public class GlassTile : GameTile
    {
        public GameObject BreakingGlassPrefab;
        [FormerlySerializedAs("_break")]
        [SerializeField] private SoundData _breakSFX;
        [SerializeField] private SoundData _stepSFX;

        public override void OnEnter(Tilemap tilemap, Vector3Int pos)
        {
            SoundController.Instance.CreateSound()
                .WithSoundData(_stepSFX)
                .Play();
        }

        public override void OnExit(Tilemap tilemap, Vector3Int pos)
        {
            tilemap.SetTile(pos, null);
            Instantiate(BreakingGlassPrefab, tilemap.GetCellCenterWorld(pos), Quaternion.identity);
            SoundController.Instance.CreateSound()
                .WithSoundData(_breakSFX)
                .Play();
        }
    }
}
