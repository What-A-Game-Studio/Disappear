using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace WAG.Core.Controls
{

    public class InputManager : MonoBehaviour
    {
        [SerializeField] private ControlMap startMap = ControlMap.Menu;
        public static InputManager Instance { get; private set; }

        private PlayerInput playerInput;
        private InputActionMap InGameMap;

        #region In Game Controls

        public Vector2 Move { get; private set; }
        public Vector2 Look { get; private set; }
        public bool Run { get; private set; }
        public bool Jump { get; private set; }
        public bool Crouch { get; private set; }
        public bool Use { get; private set; }
        public bool OpenMenu { get; private set; }


        private InputAction moveAction;
        private InputAction lookAction;
        private InputAction runAction;
        private InputAction jumpAction;
        private InputAction couchAction;
        private InputAction openInventoryAction;
        private InputAction useAction;
        private InputAction openMenuAction;

        #endregion In Game Controls

        #region Inventory Controls

        public bool Discard { get; private set; }
        public bool Rotate { get; private set; }

        private InputAction discardAction;
        private InputAction rotateAction;
        private InputAction closeInventoryAction;

        #endregion Inventory Controls

        #region UI Controls

        public bool CloseMenu { get; private set; }

        private InputAction closeMenuAction;

        #endregion UI Controls

        private void Awake()
        {
            if (InputManager.Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            if (!TryGetComponent(out playerInput))
            {
                Debug.LogError("PlayerInput required", this);
                Debug.Break();
            }

            playerInput.SwitchCurrentActionMap(startMap.ToString());
            InGameMap = playerInput.currentActionMap;

            moveAction = playerInput.actions[ActionsControls.Move.ToString()];
            lookAction = playerInput.actions[ActionsControls.Look.ToString()];
            runAction = playerInput.actions[ActionsControls.Run.ToString()];
            jumpAction = playerInput.actions[ActionsControls.Jump.ToString()];
            couchAction = playerInput.actions[ActionsControls.Crouch.ToString()];
            openInventoryAction = playerInput.actions[ActionsControls.OpenInventory.ToString()];
            useAction = playerInput.actions[ActionsControls.Use.ToString()];
            openMenuAction = playerInput.actions[ActionsControls.OpenMenu.ToString()];
            closeMenuAction = playerInput.actions[ActionsControls.CloseMenu.ToString()];
            discardAction = playerInput.actions[ActionsControls.Discard.ToString()];
            rotateAction = playerInput.actions[ActionsControls.Rotate.ToString()];
            closeInventoryAction = playerInput.actions[ActionsControls.CloseInventory.ToString()];

            moveAction.performed += OnMove;
            lookAction.performed += OnLook;
            runAction.performed += OnRun;
            jumpAction.performed += OnJump;
            couchAction.performed += OnCrouch;
            openInventoryAction.performed += OnOpenInventory;
            useAction.performed += OnUse;
            openMenuAction.performed += OnOpenMenu;
            closeMenuAction.performed += OnCloseMenu;
            discardAction.performed += OnDiscard;
            rotateAction.performed += OnRotate;
            closeInventoryAction.performed += OnCloseInventory;

            moveAction.canceled += OnMove;
            lookAction.canceled += OnLook;
            runAction.canceled += OnRun;
            jumpAction.canceled += OnJump;
            couchAction.canceled += OnCrouch;
            openInventoryAction.canceled += OnOpenInventory;
            useAction.canceled += OnUse;
            openMenuAction.canceled += OnOpenMenu;
            closeMenuAction.canceled += OnCloseMenu;
            discardAction.canceled += OnDiscard;
            rotateAction.canceled += OnRotate;
            closeInventoryAction.canceled += OnCloseInventory;
        }

        #region Private Callback Methods

        private void OnEnable()
        {
            InGameMap.Enable();
        }

        private void OnDisable()
        {
            InGameMap?.Disable();
        }

        private void OnMove(InputAction.CallbackContext context)
        {
            Move = context.ReadValue<Vector2>();
        }

        private void OnLook(InputAction.CallbackContext context)
        {
            Look = context.ReadValue<Vector2>();
        }

        private void OnRun(InputAction.CallbackContext context)
        {
            Run = context.ReadValueAsButton();
        }

        private void OnJump(InputAction.CallbackContext context)
        {
            Jump = context.ReadValueAsButton();
        }

        private void OnCrouch(InputAction.CallbackContext context)
        {
            Crouch = context.ReadValueAsButton();
        }

        private void OnOpenInventory(InputAction.CallbackContext context)
        {
            playerInput.SwitchCurrentActionMap(ControlMap.Inventory.ToString());
        }

        private void OnCloseInventory(InputAction.CallbackContext context)
        {
            playerInput.SwitchCurrentActionMap(ControlMap.Player.ToString());
        }

        private void OnOpenMenu(InputAction.CallbackContext context)
        {
            playerInput.SwitchCurrentActionMap(ControlMap.Pause.ToString());
        }

        private void OnCloseMenu(InputAction.CallbackContext context)
        {
            playerInput.SwitchCurrentActionMap(ControlMap.Player.ToString());
        }

        private void OnUse(InputAction.CallbackContext context)
        {
            Use = context.ReadValueAsButton();
        }

        private void OnDiscard(InputAction.CallbackContext context)
        {
            Discard = context.ReadValueAsButton();
        }

        private void OnRotate(InputAction.CallbackContext context)
        {
            Rotate = context.ReadValueAsButton();
        }

        #endregion Private Callback Methods

        #region Public Methods

        /// <summary>
        /// Add event on specific events
        /// </summary>
        /// <param name="actionControl">Event to add events</param>
        /// <param name="performed">
        /// Event that is triggered when the action has been <see cref="started"/>
        /// but then canceled before being fully <see cref="performed"/>.
        /// </param>
        /// <param name="started"> Event that is triggered when the action has been started.</param>
        /// <param name="canceled">Event that is triggered when the action has been fully performed.</param>
        public void AddCallbackAction(ActionsControls actionControl,
            Action<InputAction.CallbackContext> performed,
            Action<InputAction.CallbackContext> started = null,
            Action<InputAction.CallbackContext> canceled = null)
        {
            if (started != null)
                playerInput.actions[actionControl.ToString()].started += started;
            if (performed != null)
                playerInput.actions[actionControl.ToString()].performed += performed;
            if (canceled != null)
                playerInput.actions[actionControl.ToString()].canceled += canceled;
        }
        /// <summary>
        /// remove event on specific events
        /// </summary>
        /// <param name="actionControl">Event to add events</param>
        /// <param name="performed">
        /// Event that is triggered when the action has been <see cref="started"/>
        /// but then canceled before being fully <see cref="performed"/>.
        /// </param>
        /// <param name="started"> Event that is triggered when the action has been started.</param>
        /// <param name="canceled">Event that is triggered when the action has been fully performed.</param>
        public void RemoveCallbackAction(ActionsControls actionControl,
            Action<InputAction.CallbackContext> performed,
            Action<InputAction.CallbackContext> started = null,
            Action<InputAction.CallbackContext> canceled = null)
        {
            if (started != null)
                playerInput.actions[actionControl.ToString()].started -= started;
            if (performed != null)
                playerInput.actions[actionControl.ToString()].performed -= performed;
            if (canceled != null)
                playerInput.actions[actionControl.ToString()].canceled -= canceled;
        }
        public void SwitchMap(ControlMap map)
        {
            if (playerInput.actions.FindActionMap(map.ToString()) != null)
            {
                playerInput.SwitchCurrentActionMap(map.ToString());
            }
        }

        #endregion Public Methods
    }
}