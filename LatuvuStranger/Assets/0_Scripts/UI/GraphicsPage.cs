using System.Collections.Generic;
using Latuvu._0_Scripts.UI;
using UnityEngine;
using UnityEngine.UIElements;

namespace Latuvu
{
    public class GraphicsPage : UIView
    {
        #region Fields

        private DropdownField _resolutionDropdown;
        private Toggle _fullScreenToggle;
        public Button BackButton;
        
        private List<Vector2Int> _availableResolutions;
        private List<string> _resolutionLabels;
        private int _currentResIndex;

        #endregion
        
        public GraphicsPage(VisualElement root)
        {
            _hideOnAwake = true;
            _isOverlay = true;

            _resolutionDropdown = root.Q<DropdownField>("resolution");
            _fullScreenToggle = root.Q<Toggle>("fullscreen");
            BackButton = root.Q<Button>("back-button");

            SetupResolutions();
            
            if (_fullScreenToggle != null)
            {
                _fullScreenToggle.focusable = true;
                _fullScreenToggle.value = Screen.fullScreen;
                _fullScreenToggle.RegisterValueChangedCallback(OnFullScreenChanged);
            }
            
            Initialize(root);
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
        
        private void ApplyResolution(int index)
        {
            if (_availableResolutions == null || index < 0 || index >= _availableResolutions.Count)
                return;

            var res = _availableResolutions[index];
            Screen.SetResolution(res.x, res.y, Screen.fullScreen);

            if (_resolutionLabels != null && index < _resolutionLabels.Count)
                _resolutionDropdown.value = _resolutionLabels[index];
        }
        
        private void OnFullScreenChanged(ChangeEvent<bool> ev)
        {
            ApplyFullScreen(ev.newValue);
        }

        private void ApplyFullScreen(bool fullscreen)
        {
            Screen.fullScreen = fullscreen;
        }

        public override void Dispose()
        {
            if (_fullScreenToggle != null)
                _fullScreenToggle.UnregisterValueChangedCallback(OnFullScreenChanged);
        }
    }
}
