using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace TigerFrogGames.Core
{
    public class UIRebindElement : MonoBehaviour
    {
        public event Action<TMP_Text, string, string> OnUIUpdate;
        
        /* ------- Variables ------- */
        [SerializeField] private TMP_Text m_ActionLabel;
        [SerializeField] private TMP_Text m_BindingText;
       
        [SerializeField] private Button m_rebindButton;
        [SerializeField] private Button m_resetButton;

        [SerializeField] private GameObject overLayObject;
        [SerializeField] private TMP_Text overLayText;

        [SerializeField] private InputReaderExample inputReaderExample;
        
        private InputControlScheme DisplayScheme;
        
        public InputBinding.DisplayStringOptions displayStringOptions
        {
            get => m_DisplayStringOptions;
            set
            {
                m_DisplayStringOptions = value;
                UpdateBindingDisplay();
            }
        }
        [SerializeField] private InputBinding.DisplayStringOptions m_DisplayStringOptions;
        
        
        [FormerlySerializedAs("reference")] [SerializeField] private InputActionReference inputActionReference;
        
        /* ------- Unity Methods ------- */



        /* ------- Methods ------- */
    
        private void OnEnable()
        {
            m_rebindButton.onClick.AddListener(StartInteractiveRebind);
            m_resetButton.onClick.AddListener(ResetBindingsToDefault);
        }

        
        private void OnDisable()
        {
            m_rebindButton.onClick.RemoveListener(StartInteractiveRebind);
            m_resetButton.onClick.RemoveListener(ResetBindingsToDefault);
        }
    
        public void SetUp(InputActionReference inputActionReferenceIn, InputReaderExample inputReaderExampleIn , InputControlScheme currentScheme, GameObject overLayObjectIn, TMP_Text overlayTextIn)
        {
            inputReaderExample = inputReaderExampleIn;
            inputActionReference = inputActionReferenceIn;
            
            gameObject.name = $"Rebind UI - {inputActionReference.name}";
            
            overLayObject = overLayObjectIn;
            overLayText = overlayTextIn;
            
            ChangeScheme(currentScheme);
            
            UpdateActionLabel();
            UpdateBindingDisplay();
        }
        
        private void UpdateActionLabel()
        {
            if (m_ActionLabel != null)
            {
                m_ActionLabel.text = inputActionReference.action.name;
            }
        }
        
        public void UpdateBindingDisplay()
        {
          string displayString = inputReaderExample.GetBindingDisplayString(
                inputActionReference.name,
                DisplayScheme ,
                displayStringOptions,
                out string deviceLayoutName,
                out string controlPath );
           
            
            if (m_BindingText != null)
            {
                m_BindingText.text = displayString;
            }
            
            OnUIUpdate?.Invoke(m_BindingText, displayString, DisplayScheme.ToString());
        }
        
        public void ChangeScheme(InputControlScheme currentScheme)
        {
            DisplayScheme = currentScheme;
            UpdateBindingDisplay();
        }
        
        private void StartInteractiveRebind()
        {
            overLayObject?.SetActive(true);

            if (overLayObject != null && overLayText != null && inputReaderExample.isBindingInProgress() == false &&
                m_BindingText != null)
            {
                m_BindingText.text = "<Waiting...>";
            }
            
            inputReaderExample.StartInteractiveRebind(inputActionReference.action.name, DisplayScheme.ToString(), RebindStarted , RebindCompleted, RebindCanceled);
        }

        private void RebindStarted(string displayString)
        {
            overLayText.text = displayString;
        }


        private void ResetBindingsToDefault()
        {
            inputReaderExample.ResetBindingsToDefault(inputActionReference.action.name, DisplayScheme.ToString());
        }
        
        private void RebindCompleted()
        {
            if (overLayObject != null)
                overLayObject.SetActive(false);
            UpdateBindingDisplay();
        }

        private void RebindCanceled()
        {
            if (overLayObject != null)
                overLayObject.SetActive(false);
            UpdateBindingDisplay();
        }
        
        private void OnValidate()
        {
            if(inputReaderExample == null) return;
            
            UpdateActionLabel();
            UpdateBindingDisplay();
        }
    }
}