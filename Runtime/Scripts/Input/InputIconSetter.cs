using TMPro;
using UnityEngine;

namespace TigerFrogGames.Core
{
    public class InputIconSetter : MonoBehaviour
    {
        /* ------- Variables ------- */

       [SerializeField] private KeyBindingReBinder keyBindingReBinder;
       [SerializeField] private TMP_SpriteAsset spriteAssetMouseAndKeyboard;
       
       private string spriteAssetToUse = "";
        /* ------- Unity Methods ------- */

        private void Awake()
        {
            foreach (UIRebindElement uiRebindElement in keyBindingReBinder.GetRebindUIElements())
            {
                uiRebindElement.OnUIUpdate += UiRebindElementOnOnUIUpdate;
            }

            spriteAssetToUse = spriteAssetMouseAndKeyboard.name;
        }

        private void OnDestroy()
        {
            foreach (UIRebindElement uiRebindElement in keyBindingReBinder.GetRebindUIElements())
            {
                uiRebindElement.OnUIUpdate -= UiRebindElementOnOnUIUpdate;
            }
        }

        /* ------- Methods ------- */
        
        private void UiRebindElementOnOnUIUpdate(TMP_Text uiRebindTextElement,  string displayStringBase, string deviceLayoutString)
        {
            string displayString = "";
            
            string[] tokens = displayStringBase.Split('/');

            for (int i = 0; i < tokens.Length; i++)
            {
                int index = spriteAssetMouseAndKeyboard.GetSpriteIndexFromName(tokens[i]);
                
                if (index != -1)
                {
                    displayString +=  $"<sprite=\"{spriteAssetToUse}\" index={index}>";
                }
                else
                {
                    displayString += tokens[i];
                }

                if (i != tokens.Length - 1)
                {
                    displayString += " - ";
                }
            }
            uiRebindTextElement.text = displayString;
            
            //Changed to use a single sprite sheet for all controller types
            /*switch (deviceLayoutString)
            {
                case "Keyboard":
                case "Mouse":
                    index = spriteAssetMouseAndKeyboard.GetSpriteIndexFromName(displayString);
                    spriteAssetToUse = spriteAssetMouseAndKeyboard.name;
                    break;
                case "Gamepad":
                    break;
                default:
                    break;
            }*/

        }

             
        
    }
}