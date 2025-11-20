using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace Latuvu._0_Scripts.UI
{
    [RequireComponent(typeof(UIDocument))]
    public class MainMenu : MonoBehaviour
    {
        private List<Button> _menuButtons;
        private UIDocument _uiDoc;
        private VisualElement _mainMenu, _image;
        private Settings _settings;
        private VisualElement _root;
        
        [SerializeField] private List<Sprite> _menuImages;
        [SerializeField] private List<Sprite> _settingsImages;

        void OnEnable()
        {
            _uiDoc = GetComponent<UIDocument>();
            _root = _uiDoc.rootVisualElement;
            
            _mainMenu = _root.Q<VisualElement>("MainMenu");
            _image = _root.Q<VisualElement>("ImageMenu");
            _settings = new Settings(_root.Q<VisualElement>("SettingsMenu"));

            _menuButtons = new List<Button>
            {
                _root.Q<Button>("StartGame"),
                _root.Q<Button>("Settings"),
                _root.Q<Button>("Quit")
            };

            foreach (var b in _menuButtons)
            {
                if (b != null)
                    b.focusable = true;
            }
            
            _menuButtons[0].Focus();
            
            _image.style.backgroundImage = new StyleBackground(_menuImages[0]);
            _settings.SetupFocus(_settingsImages);
            _settings.Buttons[4].clicked += () => BackMenu();
            
            _menuButtons[0].clicked += StartGame;
            _menuButtons[1].clicked += OpenSettings;
            _menuButtons[2].clicked += QuitGame;
            
            for (int i = 0; i < _menuButtons.Count; i++)
            {
                _menuButtons[i].RegisterCallback<FocusInEvent> (ev =>
                {
                    _image.style.backgroundImage = new StyleBackground(_menuImages[_menuButtons.IndexOf(ev.target as Button)]);
                });
            }
        }

        void StartGame()
        {
            //SceneManager.LoadScene("GameScene");
            Debug.Log("Starting game");
        }
        
        void OpenSettings()
        {
            // Open settings menu
            _settings.Show();
            _mainMenu.RemoveFromClassList("visible");
            _mainMenu.AddToClassList("hidden");
        }
        
        void QuitGame()
        {
            Application.Quit();
        }

        private void BackMenu()
        {
            _settings.Hide();
            _mainMenu.RemoveFromClassList("hidden");
            _mainMenu.AddToClassList("visible");
        }

        void OnDisable()
        {
            if (_menuButtons != null)
            {
                if (_menuButtons.Count > 0) _menuButtons[0].clicked -= StartGame;
                if (_menuButtons.Count > 1) _menuButtons[1].clicked -= OpenSettings;
                if (_menuButtons.Count > 2) _menuButtons[2].clicked -= QuitGame;
            }
        }
    }
}
