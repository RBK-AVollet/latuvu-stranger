using System.Collections.Generic;
using Latuvu._0_Scripts.UI;
using UnityEngine;
using UnityEngine.UIElements;

namespace Latuvu
{
    public class Settings : UIView
    {
        private const string k_volumeSaveKey = "Volume";
        
        public Settings(VisualElement root) 
        {
            _hideOnAwake = true;
            _isOverlay = true;
            
            _imageMenu = root.Q<VisualElement>("image-menu");
            
            _audioPage = new AudioPage(root.Q<VisualElement>("audio-page"));
            _graphicsPage = new GraphicsPage(root.Q<VisualElement>("graphics-page"));
            _settingsMainPage = root.Q<VisualElement>("settings-mainpage");
            
            _settingsButtons = new List<Button>()
            {
                root.Q<Button>("graphics"),
                root.Q<Button>("audio"),
                root.Q<Button>("controls"),
                root.Q<Button>("language"),
                root.Q<Button>("back-button")
            };
            
            _settingsButtons[0].clicked += () => OpenPage(_graphicsPage);
            _settingsButtons[1].clicked += () => OpenPage(_audioPage);
            
            _audioPage.BackButton.clicked += () => ReturnToSettingsSelection();
            _graphicsPage.BackButton.clicked += () => ReturnToSettingsSelection();
            
            Initialize(root);
        }
        
        private void OpenPage(UIView page)
        {
            _settingsMainPage.style.display = DisplayStyle.None;
            page.Show();
        }
        
        private void ReturnToSettingsSelection()
        {
            _settingsMainPage.style.display = DisplayStyle.Flex;
            _settingsButtons[0].Focus();
            _audioPage.Hide();
            _graphicsPage.Hide();
        }

        #if !UNITY_EDITOR
        public void LoadSaveData()
        {
            const string k_fullscreenSaveKey = "Fullscreen";
            const string k_resolutionSaveKey = "Resolution";
            const string k_volumeSaveKey = "Volume";

            Vector2Int res = ES3.Load(k_resolutionSaveKey, new Vector2Int(1920, 1080));
            bool fullscreen = ES3.Load<bool>(k_fullscreenSaveKey, true);
            Screen.SetResolution(res.x, res.y, fullscreen);
            
            AudioListener.volume = ES3.Load<float>(k_volumeSaveKey, 1f);
        }
        #endif

        public override void Show()
        {
            base.Show();
            _settingsButtons[0].Focus();
        }

        public override void Dispose()
        {
            
        }
        
        #region Fields
        // Add fields here as needed
        
        private AudioPage _audioPage;
        private GraphicsPage _graphicsPage;
        
        private List<Button> _settingsButtons;

        private VisualElement _imageMenu;
        private VisualElement _settingsMainPage;
        
        public List<Button> Buttons => _settingsButtons;
        public VisualElement ImageMenu => _imageMenu;
        
        #endregion
    }
}
