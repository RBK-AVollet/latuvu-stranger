using System;
using System.Collections;
using System.Collections.Generic;
using Latuvu._0_Scripts.UI;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

namespace Latuvu
{
    public class GameUIMgr : Singleton<GameUIMgr>
    {
        private List<Button> _buttons;
        private UIDocument _uiDoc;
        private VisualElement _root;
        private UIView _currentView;
        
        private Settings _settings;
        private PauseMenu _pauseMenu;
        private Memories _memories;
        
        [SerializeField] private List<Sprite> _pauseMenuImages;
        
        private DialogueUI _dialogueUI;
        float typewriterDelay = 0.05f;
        [SerializeField] private string _debugString;
        private bool _isWriting;

        private void Awake()
        {
            _uiDoc = GetComponent<UIDocument>();
            _root = _uiDoc.rootVisualElement;
            _pauseMenu = new PauseMenu(_root.Q<VisualElement>("pause-menu"));
            
            _pauseMenu.MenuImage.style.backgroundImage = new StyleBackground(_pauseMenuImages[0]);
            
            _currentView = _pauseMenu;
            
            _buttons = _pauseMenu.Buttons;
            
            _dialogueUI = new DialogueUI(_root.Q<VisualElement>("dialogue"));

            foreach (var b in _buttons)
            {
                if (b != null)
                    b.focusable = true;
            }
            
            _buttons[0]?.Focus();
            
            _buttons[0].clicked += Resume;
            _buttons[1].clicked += () => OpenPage(_memories);
            _buttons[2].clicked += () => OpenPage(_settings);
            
            for (int i = 0; i < _pauseMenu.Buttons.Count; i++)
            {
                _pauseMenu.Buttons[i].RegisterCallback<FocusInEvent> (ev =>
                {
                    _pauseMenu.MenuImage.style.backgroundImage = new StyleBackground(_pauseMenuImages[_pauseMenu.Buttons.IndexOf(ev.target as Button)]);
                });
            }

            _settings = new Settings(_root.Q<VisualElement>("settings-menu"));
            _settings.BackButton.clicked += () => BackMenu();
            
            _memories = new Memories(_root.Q<VisualElement>("memories-menu"));
            //_memories.BackButton.clicked += () => BackMenu();

            ShowDialogue("This is a Test");
            
            OpenPauseMenu();
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

        public void OpenPauseMenu()
        {
            if (_isWriting)
                return;
            
            Time.timeScale = 0;
            _root.style.backgroundColor = new StyleColor(new Color(0, 0, 0, 1f));
            _pauseMenu.Show();
        }

        private void Resume()
        {
            Time.timeScale = 1;
            _root.style.backgroundColor = new StyleColor(new Color(0, 0, 0, 0f));
            _pauseMenu.Hide();
        }

        public void ShowDialogue(string text)
        {
            _dialogueUI.Show();
            StartCoroutine(WriteText(text));
        }
        
        public void CloseDialogue()
        {
            _dialogueUI.Hide();
        }
        
        IEnumerator WriteText(string text)
        {
            for (var i = 0; i < text.Length; i++)
            {
                var visibleText = text[..(i + 1)];
                var invisibleText = text[(i + 1)..];
                _dialogueUI.DialogueText.text = $"{visibleText}<alpha=#00>{invisibleText}";
                _debugString = _dialogueUI.DialogueText.text;
                _isWriting = true;
        
                yield return new WaitForSeconds(typewriterDelay);
            }
            
            _isWriting = false;
        }
    }
}
