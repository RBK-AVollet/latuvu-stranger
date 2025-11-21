using Latuvu._0_Scripts.UI;
using UnityEngine;
using UnityEngine.UIElements;

namespace Latuvu
{
    public class AudioPage : UIView
    {
        #region Fields

        private Button _masterVolumeButton;
        private Slider _masterVolumeSlider;
        private Label _masterVolumeLabel;
        public Button BackButton;

        #endregion
        public AudioPage(VisualElement root)
        {
            _hideOnAwake = true;
            _isOverlay = true;

            _masterVolumeButton = root.Q<Button>("master");
            _masterVolumeSlider = root.Q<Slider>("master-slider");
            _masterVolumeLabel = root.Q<Label>("volume-level");
            BackButton = root.Q<Button>("back-button");
            
            if (_masterVolumeSlider != null)
            {
                _masterVolumeSlider.focusable = true;
                _masterVolumeSlider.RegisterValueChangedCallback(evt =>
                {
                    float volume = evt.newValue;
                    AudioListener.volume = volume;
                    if (_masterVolumeLabel != null)
                    {
                        _masterVolumeLabel.text = $"Master Volume: {(int)(volume * 100)}%";
                    }
                });
                
                float currentVolume = AudioListener.volume;
                _masterVolumeSlider.value = currentVolume;
                if (_masterVolumeLabel != null)
                {
                    _masterVolumeLabel.text = $"Master Volume: {(int)(currentVolume * 100)}%";
                }
            }
            
            Initialize(root);
        }
        
        public override void Dispose()
        {
            if (_masterVolumeSlider != null)
            {
                _masterVolumeSlider.UnregisterValueChangedCallback(null);
            }
        }
    }
}
