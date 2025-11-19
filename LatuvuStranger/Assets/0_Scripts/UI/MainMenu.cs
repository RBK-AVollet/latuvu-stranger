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
            
            _settings.BackButton.clicked += () => BackMenu();
            
            _menuButtons[0]?.Focus();
            
            root.RegisterCallback<KeyDownEvent>(OnKeyDown);
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
            if (_uiDoc != null)
                _uiDoc.rootVisualElement.UnregisterCallback<KeyDownEvent>(OnKeyDown);
            
            _menuButtons[0].clicked -= StartGame;
            _menuButtons[1].clicked -= OpenSettings;
            _menuButtons[2].clicked -= QuitGame;
        }

        private void OnKeyDown(KeyDownEvent ev)
        {
            if (ev.keyCode == KeyCode.UpArrow || ev.keyCode == KeyCode.DownArrow)
            {
                var focused = _uiDoc.rootVisualElement.panel?.focusController?.focusedElement as Button;
                int current = _menuButtons.IndexOf(focused);

                int dir = 0;
                if (ev.keyCode == KeyCode.DownArrow) dir = -1;
                if (ev.keyCode == KeyCode.UpArrow) dir = 1;

                int next;
                if (current >= 0)
                    next = (current + dir + _menuButtons.Count) % _menuButtons.Count;
                else
                    next = dir == 1 ? 0 : _menuButtons.Count - 1;

                _menuButtons[next]?.Focus();
                ev.StopPropagation();
                return;
            }
            
            if (ev.keyCode == KeyCode.W || ev.keyCode == KeyCode.KeypadEnter)
            {
                var focusedButton = _uiDoc.rootVisualElement.panel?.focusController?.focusedElement as Button;
                if (focusedButton != null)
                {
                    ev.StopPropagation();
                }
            }
        }
    }
}
