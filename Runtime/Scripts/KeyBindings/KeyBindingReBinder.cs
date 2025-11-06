using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace TigerFrogGames.Core
{
    public class KeyBindingReBinder : MonoBehaviour
    {
        /* ------- Variables ------- */

        
       [SerializeField] private InputReaderExample inputReaderExample;
       [SerializeField] private InputActionAsset inputActionAsset;
       [SerializeField] private InputActionReference[] myPlayerInputsToBind;
       
       [SerializeField] private UIRebindElement prefabRebind;
       [SerializeField] private UITextLabel prefabLabel;
       [SerializeField] private GameObject uiRebindElementRoot;
       
       [SerializeField] private TMP_Text uiTextSchemeLabel;
       [SerializeField] private Button leftSchemeChangeButton;
       [SerializeField] private Button rightSchemeChangeButton;
       
       [SerializeField] private GameObject uiRebindOverLay;
       [SerializeField] private TMP_Text uiRebindOverLayText;
       
              
       [SerializeField] private List<UIRebindElement> UIRebindElements = new();
       
       [SerializeField] private List<String> mapNameLabelsAdded = new();
       [SerializeField] private List<UITextLabel> UIRebindLabelElements = new();
       
       private ReadOnlyArray<InputControlScheme> allSchemes;
       private int currentSchemeIndex = 0;
       
        /* ------- Unity Methods ------- */
        /*
        private void Start()
        {
            SetUpUIElements();
        }
        */
        
        private void Start()
        {
            InputSystem.onActionChange += OnActionChange;

            leftSchemeChangeButton.onClick.AddListener(delegate { ChangeLabel(1); });
            rightSchemeChangeButton.onClick.AddListener(delegate { ChangeLabel(-1); });
            
            allSchemes = inputReaderExample.GetControlSchemes();
            currentSchemeIndex = 0;
            
            UpdateScheme();
        }
        
        private void OnDisable()
        {
            InputSystem.onActionChange -= OnActionChange;
            
            leftSchemeChangeButton.onClick.RemoveListener(delegate { ChangeLabel(1); });
            rightSchemeChangeButton.onClick.RemoveListener(delegate { ChangeLabel(-1); });
        }
        
        /* ------- Methods ------- */

        public List<UIRebindElement> GetRebindUIElements() => UIRebindElements;
        
        private void UpdateScheme()
        {
            uiTextSchemeLabel.text = allSchemes[currentSchemeIndex].name;
            
            foreach (UIRebindElement uiRebindElement in UIRebindElements)
            {
                uiRebindElement.ChangeScheme(allSchemes[currentSchemeIndex]);
            }
        }
        
        private void ChangeLabel(int i)
        {
            currentSchemeIndex += i;
            
            if(currentSchemeIndex >= allSchemes.Count) currentSchemeIndex = 0;
            if(currentSchemeIndex < 0) currentSchemeIndex = allSchemes.Count - 1;
            
            UpdateScheme();
        }
        
        private void OnActionChange(object obj, InputActionChange change)
        {
            if(change != InputActionChange.BoundControlsChanged)
                return;

          //  var action = obj as InputAction;
           // var actionMap = action?.actionMap ?? obj as InputActionMap;
           // var actionAsset = actionMap?.asset ?? obj as InputActionAsset;
            
            for(var i = 0; i < UIRebindElements.Count; ++i)
            {
                UIRebindElements[i].UpdateBindingDisplay();
            }
        }
        

        
        
#if UNITY_EDITOR
        
        /* ------- Editor Methods------- */
        
        [ContextMenu("Print Rebind IDs")]
        private void PrintRebindIDs()
        {
            inputReaderExample.PrintBindingIds();
            
        }
        
        private void OnValidate()
        {
            if (inputReaderExample == null)
            {
                Debug.LogWarning("Input Reader is null.");
                return;
            }

            allSchemes = inputActionAsset.controlSchemes;

            //UpdateScheme();

        }
        
        [ContextMenu("Set up Rebind Elements")]
        private void SetUpUIElements()
        {
            for (int i = UIRebindElements.Count - 1; i >= 0; i--)
            {
                if (UIRebindElements[i] != null)
                {
                    if (Application.isPlaying)
                    {
                        Destroy(UIRebindElements[i].gameObject);
                    }
                    else
                    {
                        DestroyImmediate(UIRebindElements[i].gameObject);
                    }
                }
            }

            for (int i = UIRebindLabelElements.Count - 1; i >= 0; i--)
            {
                if (UIRebindLabelElements[i] != null)
                {
                    if (Application.isPlaying)
                    {
                        Destroy(UIRebindLabelElements[i].gameObject);
                    }
                    else
                    {
                        DestroyImmediate(UIRebindLabelElements[i].gameObject);
                    }
                }
            }

            UIRebindElements = new();
            
            UIRebindLabelElements = new ();
            mapNameLabelsAdded = new ();

            
            foreach (InputActionReference inputActionToRebind in myPlayerInputsToBind)// inputReader.GetAllActions())
            {
                
                if (!mapNameLabelsAdded.Contains( inputActionToRebind.action.actionMap.name))
                {
                    string label = inputActionToRebind.action.actionMap.name;

                    mapNameLabelsAdded.Add(label);
                    var createdUILabel = Instantiate(prefabLabel, uiRebindElementRoot.transform);
                    createdUILabel.SetLabel(label);
                    UIRebindLabelElements.Add(createdUILabel);
                }

                var createdRebindUIElement = Instantiate(prefabRebind, uiRebindElementRoot.transform);

                createdRebindUIElement.SetUp(inputActionToRebind, inputReaderExample, allSchemes[currentSchemeIndex],
                    uiRebindOverLay, uiRebindOverLayText);

                UIRebindElements.Add(createdRebindUIElement);
            }
            
        }

        [ContextMenu("Rebind UI Elements")]
        private void RebindUIElements()
        {
            UIRebindElements = new();
            UIRebindLabelElements = new ();
            mapNameLabelsAdded = new ();

            UIRebindElements.AddRange(uiRebindElementRoot.GetComponentsInChildren<UIRebindElement>());

            var foundLabels = uiRebindElementRoot.GetComponentsInChildren<UITextLabel>();

            UIRebindLabelElements.AddRange(foundLabels);
            
            foreach (var foundLabel in foundLabels)
            {
                if (!mapNameLabelsAdded.Contains(foundLabel.GetTextValue()))
                {
                    mapNameLabelsAdded.Add(foundLabel.GetTextValue());
                }
            }
        }
#endif
    }
    

    
}