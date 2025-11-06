using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace TigerFrogGames.Core
{
    public class KeyBindingSettingsLoader : MonoBehaviour
    {
        /* ------- Variables ------- */
        [FormerlySerializedAs("inputReader")]
        [SerializeField] private InputReader<MyPlayerInputs> inputReaderExample;


        /* ------- Unity Methods ------- */

        private void Start()
        {
            string rebinds = PlayerPrefs.GetString("Rebinds");
            if (!string.IsNullOrEmpty(rebinds))
            {
                inputReaderExample.SetKeyBindings(rebinds);
            }
        }

        private void OnDestroy()
        {
            PlayerPrefs.SetString("Rebinds",inputReaderExample.GetSavedKeyBindings());
        }

        /* ------- Methods ------- */


    }
}