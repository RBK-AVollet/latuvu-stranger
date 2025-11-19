using Latuvu._0_Scripts.UI;
using UnityEngine;
using UnityEngine.UIElements;

namespace Latuvu
{
    public class Settings : UIView
    {
        public Settings(VisualElement root) 
        {
            _hideOnAwake = true;
            _isOverlay = true;
            
            Initialize(root);
        }
        
        protected override void SetVisualElements()
        {
            _fullScreenToggle = Root.Q<Toggle>("fullscreen-toggle");
            _volumeSlider = Root.Q<Slider>("volume-slider");
            BackButton = Root.Q<Button>("back-button");
            
            if (_fullScreenToggle != null)
            {
                _fullScreenToggle.value = Screen.fullScreen;
                _fullScreenToggle.RegisterValueChangedCallback(evt =>
                {
                    Screen.fullScreen = evt.newValue;
                });
            }
            
            if (_volumeSlider != null)
            {
                _volumeSlider.value = AudioListener.volume;
                _volumeSlider.RegisterValueChangedCallback(evt =>
                {
                    AudioListener.volume = evt.newValue;
                });
            }
        }
        
        
        public override void Dispose()
        {
            
        }
        
        #region Fields
        // Add fields here as needed
        
        private Toggle _fullScreenToggle;
        private Slider _volumeSlider;
        
        public Button BackButton;
        
        #endregion
    }
}
