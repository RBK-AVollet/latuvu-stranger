using System;
using System.Collections;
using System.Collections.Generic;
using Latuvu._0_Scripts.UI;
using UnityEngine;
using UnityEngine.InputSystem;
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
        [SerializeField] private List<Sprite> _settingsImages;
        
        private DialogueUI _dialogueUI;
        float typewriterDelay = 0.05f;
        [SerializeField] private string _debugString;
        private bool _dialogueOpen = false;

        private VisualElement _gravure;
        private InputService _inputService;

        private void Awake()
        {
            _uiDoc = GetComponent<UIDocument>();
            _root = _uiDoc.rootVisualElement;
            _pauseMenu = new PauseMenu(_root.Q<VisualElement>("pause-menu"));
            
            _pauseMenu.MenuImage.style.backgroundImage = new StyleBackground(_pauseMenuImages[0]);
            
            _currentView = _pauseMenu;
            
            _buttons = _pauseMenu.Buttons;
            
            _dialogueUI = new DialogueUI(_root.Q<VisualElement>("dialogue"));
            
            _gravure = _root.Q<VisualElement>("gravure");

            foreach (var b in _buttons)
            {
                if (b != null)
                    b.focusable = true;
            }
            
            _buttons[0]?.Focus();
            
            _buttons[0].clicked += Resume;
            //_buttons[1].clicked += () => OpenPage(_memories);
            _buttons[2].clicked += () => OpenPage(_settings);

            _settings = new Settings(_root.Q<VisualElement>("settings-menu"));
            _settings.Buttons[4].clicked += () => BackMenu();
            
            _memories = new Memories(_root.Q<VisualElement>("memories-menu"));
            //_memories.BackButton.clicked += () => BackMenu();
            
            SetupButtons(_settingsImages, _settings.Buttons, _settings.ImageMenu);
            SetupButtons(_pauseMenuImages, _buttons, _pauseMenu.MenuImage);
        }

        private void Start()
        {
            _inputService = InputService.Instance;
            _inputService.RegisterPauseAction(OpenPauseMenu);
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

            if (_currentView == _settings)
            {
                _settings.Buttons[0]?.Focus();
            }
        }

        public void OpenPauseMenu(InputAction.CallbackContext context)
        {
            if (_dialogueOpen)
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
            _dialogueOpen = true;
            _dialogueUI.Show();
            StartCoroutine(WriteText(text));
        }
        
        public void CloseDialogue()
        {
            _dialogueUI.Hide();
            _dialogueOpen = false;
        }

        private void DisplayGravure()
        {
            _gravure.style.display = DisplayStyle.Flex;
        }
        
        private void HideGravure()
        {
            _gravure.style.display = DisplayStyle.None;
        }
        
        private void ChangeImage(List<Sprite> images, int index, VisualElement image)
        {
            AddImageTransition(image);
            image.style.backgroundImage = new StyleBackground(images[index]);
            StartCoroutine(RemoveImageTransition(image));
        }
        
        private IEnumerator RemoveImageTransition(VisualElement image)
        {
            yield return new WaitForSeconds(0.5f);
            image.RemoveFromClassList("image-out");
        }

        private void AddImageTransition(VisualElement image)
        {
            image.AddToClassList("image-out");
        }

        private void SetupButtons(List<Sprite> images, List<Button> buttons, VisualElement image)
        {
            for (int i = 0; i < buttons.Count; i++)
            {
                buttons[i].RegisterCallback<FocusInEvent> (ev =>
                {
                    ChangeImage(images, buttons.IndexOf(ev.target as Button), image);
                });
            }
        }
        
        IEnumerator WriteText(string text)
        {
            for (var i = 0; i < text.Length; i++)
            {
                var visibleText = text[..(i + 1)];
                var invisibleText = text[(i + 1)..];
                _dialogueUI.DialogueText.text = $"{visibleText}<alpha=#00>{invisibleText}";
                _debugString = _dialogueUI.DialogueText.text;
        
                yield return new WaitForSeconds(typewriterDelay);
            }
        }
    }
}
