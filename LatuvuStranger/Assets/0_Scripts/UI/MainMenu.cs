using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Latuvu._0_Scripts.UI
{
    [RequireComponent(typeof(UIDocument))]
    public class MainMenu : MonoBehaviour
    {
        private List<Button> _menuButtons;
        private UIDocument _uiDoc;
        private VisualElement _mainMenu;
        private Settings _settings;
        [SerializeField] private List<Texture> _menuImages;

        void OnEnable()
        {
            _uiDoc = GetComponent<UIDocument>();
            var root = _uiDoc.rootVisualElement;
            
            _mainMenu = root.Q<VisualElement>("MainMenu");

            _menuButtons = new List<Button>
            {
                root.Q<Button>("StartGame"),
                root.Q<Button>("Settings"),
                root.Q<Button>("Quit")
            };

            foreach (var b in _menuButtons)
            {
                if (b != null)
                    b.focusable = true;
            }
            
            _settings = new Settings(root.Q<VisualElement>("SettingsMenu"));
            #if !UNITY_EDITOR
            _settings.LoadSaveData();
            #endif            

            _settings.BackButton.clicked += () => BackMenu();
            
            _menuButtons[0]?.Focus();
            
            _menuButtons[0].clicked += StartGame;
            _menuButtons[1].clicked += OpenSettings;
            _menuButtons[2].clicked += QuitGame;
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
            _menuButtons[0].clicked -= StartGame;
            _menuButtons[1].clicked -= OpenSettings;
            _menuButtons[2].clicked -= QuitGame;
        }
    }
}
