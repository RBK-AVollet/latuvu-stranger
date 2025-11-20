using Latuvu._0_Scripts.UI;
using UnityEngine;
using UnityEngine.UIElements;

namespace Latuvu
{
    public class Memories : UIView
    {
        public Button BackButton;
        
        private Label _memoriesLabel;
        private int _memoriesCrystals;
        
        public Memories(VisualElement root) 
        {
            _memoriesLabel = root.Q<Label>("memories-label");
            
            _hideOnAwake = true;
            _isOverlay = true;
            
            Initialize(root);
        }
        
        public void AddMemoryCrystal()
        {
            _memoriesCrystals++;
            UpdateMemoriesLabel();
        }

        private void UpdateMemoriesLabel()
        {
            if (_memoriesLabel != null)
            {
                _memoriesLabel.text = $"Memories Collected: {_memoriesCrystals}";
            }
        }

        public override void Dispose()
        {
            
        }
    }
}
