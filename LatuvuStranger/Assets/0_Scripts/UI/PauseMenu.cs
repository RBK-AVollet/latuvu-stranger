using System.Collections.Generic;
using Latuvu._0_Scripts.UI;
using UnityEngine;
using UnityEngine.UIElements;

namespace Latuvu
{
    public class PauseMenu : UIView
    {
        public List<Button> Buttons;
        public VisualElement MenuImage;
        
        public PauseMenu(VisualElement root)
        {
            _hideOnAwake = false;
            
            MenuImage = root.Q<VisualElement>("PauseMenuImage");
            
            Buttons = new List<Button>
            {
                root.Q<Button>("resume"),
                root.Q<Button>("memories"),
                root.Q<Button>("settings"),
                root.Q<Button>("quit")
            };
            
            Buttons[3].clicked += ReturnToMainMenu;
            
            Initialize(root);
        }
        
        public override void Dispose()
        {
            
        }
        
        private void ReturnToMainMenu()
        {
            Time.timeScale = 0;
            UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
        }
    }
}
