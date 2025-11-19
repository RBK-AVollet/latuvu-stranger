using System;
using System.Collections.Generic;
using Latuvu._0_Scripts.UI;
using UnityEngine;
using UnityEngine.UIElements;

namespace Latuvu
{
    public class PauseMenuMgr : MonoBehaviour
    {
        private List<Button> _buttons;
        private UIDocument _uiDoc;
        private Settings _settings;
        private PauseMenu _pauseMenu;
        private Memories _memories;
        private UIView _currentView;

        private void Awake()
        {
            _uiDoc = GetComponent<UIDocument>();
            var root = _uiDoc.rootVisualElement;
            _pauseMenu = new PauseMenu(root.Q<VisualElement>("pause-menu"));
            
            _currentView = _pauseMenu;

            foreach (var b in _buttons)
            {
                if (b != null)
                    b.focusable = true;
            }

            _buttons = _pauseMenu.Buttons;
            
            _buttons[0]?.Focus();
            
            _buttons[0].clicked += OpenPauseMenu;
            _buttons[1].clicked += () => OpenPage(_memories);
            _buttons[2].clicked += () => OpenPage(_settings);

            _settings = new Settings(root.Q<VisualElement>("settings-menu"));
            _settings.BackButton.clicked += () => BackMenu();
            
            _memories = new Memories(root.Q<VisualElement>("memories-menu"));
            _memories.BackButton.clicked += () => BackMenu();
            
            root.RegisterCallback<KeyDownEvent>(OnKeyDown);
        }

        private void ChangeFocus(List<Button> buttons)
        {
            
        }

        private void BackMenu()
        {
            _currentView.Hide();
            _pauseMenu.Show();
            _currentView = _pauseMenu;
        }

        private void OpenPage(UIView view)
        {
            _currentView.Hide();
            view.Show();
            _currentView = view;
        }

        private void OpenPauseMenu()
        {
            Time.timeScale = 0;
            _uiDoc.rootVisualElement.AddToClassList("visible");
        }

        private void ClosePauseMenu()
        {
            Time.timeScale = 1;
            _uiDoc.rootVisualElement.RemoveFromClassList("visible");
        }
        
        private void OnKeyDown(KeyDownEvent ev)
        {
            if (ev.keyCode == KeyCode.UpArrow || ev.keyCode == KeyCode.DownArrow)
            {
                var focused = _uiDoc.rootVisualElement.panel?.focusController?.focusedElement as Button;
                int current = _buttons.IndexOf(focused);

                int dir = 0;
                if (ev.keyCode == KeyCode.DownArrow) dir = -1;
                if (ev.keyCode == KeyCode.UpArrow) dir = 1;

                int next;
                if (current >= 0)
                    next = (current + dir + _buttons.Count) % _buttons.Count;
                else
                    next = dir == 1 ? 0 : _buttons.Count - 1;

                _buttons[next]?.Focus();
                ev.StopPropagation();
            }
        }
    }
}
