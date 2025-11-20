using System.Collections;
using Latuvu._0_Scripts.UI;
using UnityEngine;
using UnityEngine.UIElements;

namespace Latuvu
{
    public class DialogueUI : UIView
    {
        private Label _dialogueText;
        
        public Label DialogueText => _dialogueText;
        
        public DialogueUI(VisualElement root)
        {
            _hideOnAwake = true;
            
            _dialogueText = root.Q<Label>("dialogue");
            
            Initialize(root);
        }
        
        public override void Dispose()
        {
            
        }
    }
}
