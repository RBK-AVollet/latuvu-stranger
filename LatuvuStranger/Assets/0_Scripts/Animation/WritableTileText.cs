using TMPro;
using UnityEngine;

namespace Latuvu
{
    public class WritableTileText : MonoBehaviour
    { 
        TextMeshPro _text;
        
        void Awake()
        {
            _text = GetComponent<TextMeshPro>();
        }
        
        public void Write(string content)
        {
            if (content.Length != 2)
            {
                Debug.LogError("Trying to write to a writable tile with a different amount of characters than 2 !");
                return;
            }
            
            _text.SetText(content);
        }
    }
}
