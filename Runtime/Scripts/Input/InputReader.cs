using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace TigerFrogGames.Core
{
    public abstract class InputReader<TInputAsset> : ScriptableObject, IDisposable where TInputAsset : IInputActionCollection2
    {
        [Tooltip("Event that is triggered when the way the binding is display should be updated. This allows displaying "
                 + "bindings in custom ways, e.g. using images instead of text.")]
        [SerializeField]
        private UpdateBindingUIEvent m_UpdateBindingUIEvent;

        [Tooltip("Event that is triggered when an interactive rebind is being initiated. This can be used, for example, "
                 + "to implement custom UI behavior while a rebind is in progress. It can also be used to further "
                 + "customize the rebind.")]
        [SerializeField]
        private InteractiveRebindEvent m_RebindStartEvent;
        
        [Tooltip("Event that is triggered when an interactive rebind is complete or has been aborted.")]
        [SerializeField]
        private InteractiveRebindEvent m_RebindStopEvent;
        
        private InputActionRebindingExtensions.RebindingOperation currentRebindOperation;
        
        protected TInputAsset inputActions;
        
        protected abstract void SetUpInputActions();
        
        bool IsDeviceMouse(InputAction.CallbackContext context) => context.control.device.name == "Mouse";

        #region Creation/Destruction
        
        private void OnEnable()
        {
            SetUpInputActions();
        }

        private void OnDisable()
        {
            Dispose();
        }

                
        public void Dispose()
        {
            currentRebindOperation?.Dispose();
            Destroy();
        }

        //you need to call
        //inputActions.Player.Disable();
        // for each input action asset you enable
        protected virtual void Destroy(){}
        
        #endregion
        


        
        public void EnablePlayerActions()
        {
            inputActions.Enable();
        }
        
        public string GetSavedKeyBindings()
        {
            return inputActions.SaveBindingOverridesAsJson();
        }
        
        public string GetBindingDisplayString(string actionName, 
            InputControlScheme displayScheme, 
            InputBinding.DisplayStringOptions displayStringOptions,
            out string deviceLayoutName,
            out string controlPath)
        {
            SetUpInputActions();
            
            var displayString = string.Empty;
            deviceLayoutName = null;
            controlPath = null;
            
            var action = inputActions.FindAction(actionName);
            
            if (action != null)
            {
                var bindingIndex = action.GetBindingIndex(group: displayScheme.ToString());
                if (bindingIndex != -1)
                {
                    if (action.bindings[bindingIndex].isPartOfComposite) bindingIndex--;
                    
                    displayString = action.GetBindingDisplayString(bindingIndex, out deviceLayoutName, out controlPath, displayStringOptions);
                }
            }
            
            return displayString;
        }
        
        public bool isBindingInProgress()
        {
            return m_RebindStartEvent == null;
        }
        
        public ReadOnlyArray<InputControlScheme> GetControlSchemes()
        {
            //might be null if called from OnValidate
            // SetUpInputActions();
            
            return inputActions.controlSchemes;
        }
        
        public void SetKeyBindings(string rebinds)
        {
            inputActions.LoadBindingOverridesFromJson(rebinds);
        }

        #region Rebinding

        private InputActionRebindingExtensions.RebindingOperation ReBindAction(
            string actionName, int bindingIndex,
            out InputAction actionOut,
            Action<InputActionRebindingExtensions.RebindingOperation> onCancelCallback,
            Action<InputActionRebindingExtensions.RebindingOperation> onCompleteCallback)
        {
            
            var actionToRebind = inputActions.FindAction(actionName);
            actionOut = actionToRebind;
            
            
            
            var rebindOperation = actionToRebind.PerformInteractiveRebinding(bindingIndex)
                .OnCancel(onCancelCallback)
                .OnComplete(onCompleteCallback);


            return rebindOperation;
        }

        
        public void StartInteractiveRebind(
            string actionName,
            string controlScheme, 
            Action<string> onStartedCallback,
            Action onCompleteCallback,
            Action onCancelCallback)
        {
            if (!ResolveActionAndBinding(actionName, controlScheme, out var action, out var bindingIndexIn))
                return;
            
            // If the binding is a composite, we need to rebind each part in turn.
            if (action.bindings[bindingIndexIn].isComposite)
            {
                var firstPartIndex = bindingIndexIn + 1;
                if (firstPartIndex < action.bindings.Count && action.bindings[firstPartIndex].isPartOfComposite)
                    PerformInteractiveRebind(action, firstPartIndex, onStartedCallback, onCancelCallback, onCompleteCallback, allCompositeParts: true);
            }
            else
            {
                PerformInteractiveRebind(action, bindingIndexIn, onStartedCallback ,   onCancelCallback, onCompleteCallback);
            }
        }
        
        /// <summary>
        /// Gets the acton and binding from the name and control scheme. 
        /// </summary>
        /// <returns></returns>
        private bool ResolveActionAndBinding(string actionName, string controlScheme , out InputAction action, out int bindingIndex)
        {
            bindingIndex = -1;

            action = inputActions.FindAction(actionName);
            if (action == null)
                return false;
            
            //look for the binding that matches the name
            for (int i = 0; i < action.bindings.Count; i++)
            {
                //if (string.Equals( action.bindings[i].action , actionName))
                //{
                    if (action.bindings[i].isComposite )
                    {
                        int nextIndex = i + 1;
                        if (nextIndex < action.bindings.Count)
                        {
                            if (action.bindings[nextIndex].groups != null &&
                                action.bindings[nextIndex].groups.Contains(controlScheme))
                            {
                                bindingIndex = i;
                                break;
                            }
                            
                        }
                    }else if (action.bindings[i].groups != null &&
                              action.bindings[i].groups.Contains(controlScheme))
                    {
                        bindingIndex = i;
                        break;
                    }
               // }
            }
            
            //bindingIndex = action.GetBindingIndex(group: controlScheme);
            //bindingIndex = action.bindings.IndexOf(x => String.Equals(x.name, actionName));
            if (bindingIndex == -1)
            {
                Debug.LogError($"Cannot find binding with name - '{actionName}' on '{controlScheme}'", this);
                return false;
            }
            
            return true;
        }
          
                
        private void PerformInteractiveRebind(InputAction action,
            int bindingIndexIn,
            Action<string> onStartCallBackIn,
            Action onCompleteCallbackIn,
            Action onCancelCallbackIn,
            bool allCompositeParts = false)
        {
            currentRebindOperation?.Cancel(); // Will null out currentRebindOperation.
            
            //Fixes the "InvalidOperationException: Cannot rebind action x while it is enabled" error
            action.Disable();
            
            
           
            currentRebindOperation = action.PerformInteractiveRebinding(bindingIndexIn)
                .OnCancel(OnCancelCallback())
               .OnComplete(OnCompleteCallback());

            
            
            // show the name of the rebind in the UI.
            var displayString = default(string);
            if (action.bindings[bindingIndexIn].isPartOfComposite)
            {
                displayString = $"Binding '{action.bindings[bindingIndexIn].name}' of {action.name}. ";
            }
            else
            {
                displayString =  $"Binding '{action.name}'. ";
            }
                
            
            onStartCallBackIn.Invoke(displayString);
            
            // Bring up rebind overlay, if we have one.
            
            

            // If we have no rebind overlay and no callback but we have a binding text label,
            // temporarily set the binding text label to "<Waiting>".
            

            // Give listeners a chance to act on the rebind starting.
           // m_RebindStartEvent?.Invoke(this, currentRebindOperation);

            currentRebindOperation.Start();
            
            
            Action<InputActionRebindingExtensions.RebindingOperation> OnCancelCallback()
            {
                return operation =>
                {
                    onCancelCallbackIn.Invoke();
                    CleanUp();
                };
            }
            
            Action<InputActionRebindingExtensions.RebindingOperation> OnCompleteCallback()
            {
                return operation =>
                {
                    
                    CleanUp();

                    // If there's more composite parts we should bind, initiate a rebind
                    // for the next part.
                    if (allCompositeParts)
                    {
                        var nextBindingIndex = bindingIndexIn + 1;
                        if (nextBindingIndex < action.bindings.Count &&
                            action.bindings[nextBindingIndex].isPartOfComposite)
                        {
                            PerformInteractiveRebind(action, nextBindingIndex, onStartCallBackIn, onCompleteCallbackIn,
                                onCancelCallbackIn, true);
                            return;
                        }
                    }
                    onCompleteCallbackIn.Invoke();
                };
            }
            
            void CleanUp()
            {
                currentRebindOperation?.Dispose();
                currentRebindOperation = null;
                action.Enable();
            }
        }



        public void ResetBindingsToDefault(string actionName, string controlScheme)
        {
            if (!ResolveActionAndBinding(actionName, controlScheme, out var action, out var bindingIndex))
                return;

            if (action.bindings[bindingIndex].isComposite)
            {
                // It's a composite. Remove overrides from part bindings.
                for (var i = bindingIndex + 1; i < action.bindings.Count && action.bindings[i].isPartOfComposite; ++i)
                    action.RemoveBindingOverride(i);
            }
            else
            {
                action.RemoveBindingOverride(bindingIndex);
            }
        }
        
        #endregion
        
        #region Debugging

        public void PrintBindingIds()
        {
            foreach (var binding in inputActions.bindings)
            {
                Debug.Log($"{binding.action} - {binding.name} - {binding.id} - {binding.isComposite}");
            }
        }
        

        #endregion
    }
    
    [Serializable]
    public class UpdateBindingUIEvent : UnityEvent<UIRebindElement, string, string, string>
    {
    }

    [Serializable]
    public class InteractiveRebindEvent : UnityEvent<UIRebindElement, InputActionRebindingExtensions.RebindingOperation>
    {
    }
}