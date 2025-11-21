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
                    
                    #if !UNITY_EDITOR
                    const string k_volumeSaveKey = "Volume";
                    ES3.Save(k_volumeSaveKey, volume);
                    #endif
                    
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

            _masterVolumeButton.clicked += () => FocusMasterVolumeSlider();
            
            Initialize(root);
        }
        
        public override void Show()
        {
            base.Show();
        }
        
        public void FocusMasterVolumeSlider()
        {
            _masterVolumeSlider?.Focus();
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
