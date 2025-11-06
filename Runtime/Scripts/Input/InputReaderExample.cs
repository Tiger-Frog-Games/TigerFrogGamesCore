using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace TigerFrogGames.Core
{
    using static MyPlayerInputs;
    
    [CreateAssetMenu(menuName = "ScriptableObject/InputReaders/Example Basic Input Reader")]
    public class InputReaderExample : InputReader<MyPlayerInputs>, IPlayerActions, IUIActions
    {
        public event UnityAction<Vector2> Move = delegate { };
        public event UnityAction<bool> Action1 = delegate { };
        public event UnityAction<bool> Action2 = delegate { };
        public event UnityAction<bool> Interact = delegate { };
        public event UnityAction<bool> OpenMenu = delegate { };
        public event UnityAction<bool> OpenCheatsMenu = delegate { };
        
        protected override void SetUpInputActions()
        {
            if (inputActions == null)
            {
                inputActions = new MyPlayerInputs();
                inputActions.Player.SetCallbacks(this);
                inputActions.UI.SetCallbacks(this);
            }
        }
        
        protected override void Destroy()
        {
            if (inputActions != null)
            {
                inputActions.Player.Disable();
                inputActions.UI.Disable();
            }
        }
        
        public Vector2 Direction => inputActions.Player.Move.ReadValue<Vector2>();
        
        public void OnMove(InputAction.CallbackContext context)
        {
            Move.Invoke(context.ReadValue<Vector2>());
        }

        public void OnActionOne(InputAction.CallbackContext context)
        {
            switch (context.phase) {
                case InputActionPhase.Started:
                    Action1.Invoke(true);
                    break;
                case InputActionPhase.Canceled:
                    Action1.Invoke(false);
                    break;
            }
        }

        public void OnAction2(InputAction.CallbackContext context)
        {
            switch (context.phase) {
                case InputActionPhase.Started:
                    Action2.Invoke(true);
                    break;
                case InputActionPhase.Canceled:
                    Action2.Invoke(false);
                    break;
            }
        }

        public void OnInteract(InputAction.CallbackContext context)
        {
            switch (context.phase) {
                case InputActionPhase.Started:
                    Interact.Invoke(true);
                    break;
                case InputActionPhase.Canceled:
                    Interact.Invoke(false);
                    break;
            }
        }

        public void OnOpenMenu(InputAction.CallbackContext context)
        {
            switch (context.phase) {
                case InputActionPhase.Started:
                    OpenMenu.Invoke(true);
                    break;
                case InputActionPhase.Canceled:
                    OpenMenu.Invoke(false);
                    break;
            }
        }
        
        public void OnUIMove(InputAction.CallbackContext context)
        {
            //throw new System.NotImplementedException();
        }

 
    }
}