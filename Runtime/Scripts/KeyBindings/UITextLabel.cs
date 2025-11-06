using System;
using TMPro;
using UnityEngine;

namespace TigerFrogGames.Core
{
    public class UITextLabel : MonoBehaviour
    {
        /* ------- Variables ------- */

        [SerializeField] private TMP_Text text;
        [SerializeField] private String displayText;
        /* ------- Unity Methods ------- */
        
        /* ------- Methods ------- */

        public void SetLabel(string newText)
        {
            displayText = newText;

            if (string.IsNullOrEmpty(displayText))
            {
                gameObject.name = "Text Label - Empty Text";
                text.text = "";
                return;
            }
            
            gameObject.name = $"Text Label - {displayText}";
            text.text = newText;
        }
        
        
#if UNITY_EDITOR

        private void OnValidate()
        {
            SetLabel(displayText);
        }

#endif

        public string GetTextValue()
        {
            return text.text;
        }
    }
}