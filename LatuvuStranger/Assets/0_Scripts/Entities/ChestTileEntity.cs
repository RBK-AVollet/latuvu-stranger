using System;
using Padrox.Acelab.Modules.Audio;
using UnityEngine;

namespace Latuvu
{
    public class ChestTileEntity : StaticTileEntity
    {
        public bool IsOpened { get; private set; } = false;
        
        [SerializeField] protected Sprite _closedSprite;
        [SerializeField] protected Sprite _openedSprite;
        [SerializeField] protected SpriteRenderer _itemRenderer;
        [SerializeField] protected SpriteRenderer _chestRenderer;
        [SerializeField] protected float _itemDisplayTime = 1.5f;
        [SerializeField] protected SoundData _pickupSound;
        private float _displayTimer = 0f;

        protected override void Start()
        {
            base.Start();
            
            _chestRenderer.sprite = _closedSprite;
        }

        private void Update()
        {
            if (_displayTimer <= 0f) return;
            
            _displayTimer -= Time.deltaTime;
            
            if (_displayTimer <= 0f)
            {
                _itemRenderer.enabled = false;
            }
        }

        protected override void Interact(PlayerTileEntity player)
        {
            base.Interact(player);

            if (!IsOpened)
            {
                IsOpened = true;
                Debug.Log("[ChestTileEntity]: Chest at " + transform.position + " opened by player.");
                OpenChest(player);
            }
            else
            {
                Debug.Log("[ChestTileEntity]: Chest at " + transform.position + " is already opened.");
            }
        }

        protected virtual void OpenChest(PlayerTileEntity player)
        {
            _chestRenderer.sprite = _openedSprite;
            _itemRenderer.enabled = true;
            _displayTimer = _itemDisplayTime;
            SoundController.Instance.CreateSound()
                .WithSoundData(_pickupSound)
                .Play();

            Debug.Log("SOUND");
        }
    }
}
