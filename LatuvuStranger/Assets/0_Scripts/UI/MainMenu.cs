using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace Latuvu._0_Scripts.UI
{
    [RequireComponent(typeof(UIDocument))]
    public class MainMenu : MonoBehaviour
    {
        private List<Button> menuButtons;
        [SerializeField] private UIDocument uiDoc;

        void OnEnable()
        {
            uiDoc = GetComponent<UIDocument>();
            var root = uiDoc.rootVisualElement;

            menuButtons = new List<Button>
            {
                root.Q<Button>("StartGame"),
                root.Q<Button>("Settings"),
                root.Q<Button>("Quit")
            };

            foreach (var b in menuButtons)
            {
                if (b != null)
                    b.focusable = true;
            }
            
            menuButtons[0]?.Focus();
            

            root.RegisterCallback<KeyDownEvent>(OnKeyDown);
            menuButtons[0]?.RegisterCallback<ClickEvent>(ev => StartGame());
            menuButtons[1]?.RegisterCallback<ClickEvent>(ev => OpenSettings());
            menuButtons[2]?.RegisterCallback<ClickEvent>(ev => QuitGame());
        }

        void StartGame()
        {
            //SceneManager.LoadScene("GameScene");
            Debug.Log("Starting game");
        }
        
        void OpenSettings()
        {
            // Open settings menu
            Debug.Log("Opening settings");
        }
        
        void QuitGame()
        {
            Application.Quit();
            Debug.Log("Quitting game");
        }

        void OnDisable()
        {
            if (uiDoc != null)
                uiDoc.rootVisualElement.UnregisterCallback<KeyDownEvent>(OnKeyDown);
        }

        private void OnKeyDown(KeyDownEvent ev)
        {
            if (ev.keyCode != KeyCode.UpArrow && ev.keyCode != KeyCode.DownArrow)
                return;

            var focused = uiDoc.rootVisualElement.focusController.focusedElement as Button;
            int current = menuButtons.IndexOf(focused);

            int dir = 0;

            if (ev.keyCode == KeyCode.DownArrow) dir = -1;
            if (ev.keyCode == KeyCode.UpArrow) dir = 1;

            int next;
            if (current >= 0)
                next = (current + dir + menuButtons.Count) % menuButtons.Count;
            else
                next = dir == 1 ? 0 : menuButtons.Count - 1;

            menuButtons[next]?.Focus();
            ev.StopPropagation();
        }
    }
}
