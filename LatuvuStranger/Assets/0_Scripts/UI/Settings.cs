using System.Collections.Generic;
using Latuvu._0_Scripts.UI;
using UnityEngine;
using UnityEngine.UIElements;

namespace Latuvu
{
    public class Settings : UIView
    {
        private const string k_volumeSaveKey = "Volume";
        private const string k_fullscreenSaveKey = "Fullscreen";
        private const string k_resolutionSaveKey = "Resolution";
        
        public Settings(VisualElement root) 
        {
            _hideOnAwake = true;
            _isOverlay = true;
            
            Initialize(root);
        }
        
        public void LoadSaveData()
        {
            Vector2Int res = ES3.Load(k_resolutionSaveKey, new Vector2Int(1920, 1080));
            bool fullscreen = ES3.Load<bool>(k_fullscreenSaveKey, true);
            Screen.SetResolution(res.x, res.y, fullscreen);
            
            AudioListener.volume = ES3.Load<float>(k_volumeSaveKey, 1f);
        }
        
        protected override void SetVisualElements()
        {
            _fullScreenToggle = Root.Q<Toggle>("fullscreen-toggle");
            _volumeSlider = Root.Q<Slider>("volume-slider");
            BackButton = Root.Q<Button>("back-button");
            _resolutionDropdown = Root.Q<DropdownField>("resolution-dropdown");
            
            if (_fullScreenToggle != null)
            {
                _fullScreenToggle.value = Screen.fullScreen;
                _fullScreenToggle.RegisterValueChangedCallback(evt =>
                {
                    Screen.fullScreen = evt.newValue;
                    ES3.Save(k_fullscreenSaveKey, evt.newValue);
                });
            }
            
            if (_volumeSlider != null)
            {
                _volumeSlider.value = AudioListener.volume;
                _volumeSlider.RegisterValueChangedCallback(evt =>
                {
                    AudioListener.volume = evt.newValue;
                    ES3.Save(k_volumeSaveKey, evt.newValue);
                });
            }
            
            SetupResolutions();
            
            Root.RegisterCallback<KeyDownEvent>(OnKeyDown);
        }
        
        private void SetupResolutions()
        {
            if (_resolutionDropdown == null)
                return;

            _resolutionDropdown.focusable = true;
            
            _availableResolutions = new List<Vector2Int>();
            _resolutionLabels = new List<string>();

            foreach (var r in Screen.resolutions)
            {
                var v = new Vector2Int(r.width, r.height);
                if (_availableResolutions.Contains(v))
                    continue;
                _availableResolutions.Add(v);
                _resolutionLabels.Add($"{r.width}x{r.height}");
            }

            if (_availableResolutions.Count == 0)
                return;
            
            _currentResIndex = _availableResolutions.FindIndex(v => v.x == Screen.width && v.y == Screen.height);
            if (_currentResIndex < 0) _currentResIndex = 0;

            _resolutionDropdown.choices = _resolutionLabels;
            _resolutionDropdown.value = _resolutionLabels[_currentResIndex];
        }

        private void OnKeyDown(KeyDownEvent ev)
        {
            if (_resolutionDropdown == null || _availableResolutions == null || _availableResolutions.Count == 0)
                return;

            // only handle left/right when the resolution control (or its children) is focused
            var focused = Root.panel?.focusController?.focusedElement as VisualElement;
            if (focused == null)
                return;

            if (!(_resolutionDropdown == focused || _resolutionDropdown.Contains(focused)))
                return;

            if (ev.keyCode == KeyCode.RightArrow || ev.keyCode == KeyCode.LeftArrow)
            {
                int dir = ev.keyCode == KeyCode.RightArrow ? 1 : -1;
                _currentResIndex = (_currentResIndex + dir + _availableResolutions.Count) % _availableResolutions.Count;
                ApplyResolution(_currentResIndex);
                ev.StopPropagation();
            }
        }

        private void ApplyResolution(int index)
        {
            if (_availableResolutions == null || index < 0 || index >= _availableResolutions.Count)
                return;

            var res = _availableResolutions[index];
            Screen.SetResolution(res.x, res.y, Screen.fullScreen);
            ES3.Save(k_resolutionSaveKey, res);

            if (_resolutionLabels != null && index < _resolutionLabels.Count)
                _resolutionDropdown.value = _resolutionLabels[index];
        }

        public override void Dispose()
        {
            Root.UnregisterCallback<KeyDownEvent>(OnKeyDown);
        }
        
        #region Fields
        // Add fields here as needed
        
        private Toggle _fullScreenToggle;
        private Slider _volumeSlider;
        public Button BackButton;
        
        private DropdownField _resolutionDropdown;
        private List<Vector2Int> _availableResolutions;
        private List<string> _resolutionLabels;
        private int _currentResIndex;
        
        #endregion
    }
}
