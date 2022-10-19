using System;
using UnityEngine;
using UnityEngine.InputSystem;
using WaG.Input_System.Scripts;


public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    private PlayerInput playerInput;
    private InputActionMap InGameMap;

    #region In Game Controls

    public Vector2 Move { get; private set; }
    public Vector2 Look { get; private set; }
    public bool Run { get; private set; }
    public bool Jump { get; private set; }
    public bool Crouch { get; private set; }
    public bool Catch { get; private set; }
    public bool Use { get; private set; }

    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction runAction;
    private InputAction jumpAction;
    private InputAction couchAction;
    private InputAction openInventoryAction;
    private InputAction catchAction;
    private InputAction useAction;

    #endregion In Game Controls

    #region UI Controls

    public bool Discard { get; private set; }
    public bool Rotate { get; private set; }
    
    private InputAction discardAction;
    private InputAction rotateAction;
    private InputAction closeInventoryAction;

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
        
        playerInput.SwitchCurrentActionMap("Player");
        InGameMap = playerInput.currentActionMap;
        
        moveAction = playerInput.actions[ActionsControls.Move.ToString()];
        lookAction = playerInput.actions[ActionsControls.Look.ToString()];
        runAction = playerInput.actions[ActionsControls.Run.ToString()];
        jumpAction = playerInput.actions[ActionsControls.Jump.ToString()];
        couchAction = playerInput.actions[ActionsControls.Crouch.ToString()];
        openInventoryAction = playerInput.actions[ActionsControls.OpenInventory.ToString()];
        catchAction = playerInput.actions[ActionsControls.Catch.ToString()];
        useAction = playerInput.actions[ActionsControls.Use.ToString()];
        discardAction = playerInput.actions[ActionsControls.Discard.ToString()];
        rotateAction = playerInput.actions[ActionsControls.Rotate.ToString()];
        closeInventoryAction = playerInput.actions[ActionsControls.CloseInventory.ToString()];

        moveAction.performed += OnMove;
        lookAction.performed += OnLook;
        runAction.performed += OnRun;
        jumpAction.performed += OnJump;
        couchAction.performed += OnCrouch;
        openInventoryAction.performed += OnOpenInventory;
        catchAction.performed += OnCatch;
        useAction.performed += OnUse;
        discardAction.performed += OnDiscard;
        rotateAction.performed += OnRotate;
        closeInventoryAction.performed += OnCloseInventory;

        moveAction.canceled += OnMove;
        lookAction.canceled += OnLook;
        runAction.canceled += OnRun;
        jumpAction.canceled += OnJump;
        couchAction.canceled += OnCrouch;
        openInventoryAction.canceled += OnOpenInventory;
        catchAction.canceled += OnCatch;
        useAction.canceled += OnUse;
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
        InGameMap.Disable();
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
        playerInput.SwitchCurrentActionMap("UI");
    }
    
    private void OnCloseInventory(InputAction.CallbackContext context)
    {
        playerInput.SwitchCurrentActionMap("Player");
    }
    
    private void OnCatch(InputAction.CallbackContext context)
    {
        Catch = context.ReadValueAsButton();
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

    public void AddCallbackAction(ActionsControls actionControl, Action<InputAction.CallbackContext> callback)
    {
        playerInput.actions[actionControl.ToString()].performed += callback;
        // playerInput.actions[actionControl.ToString()].canceled += callback;
    }
    

    #endregion Public Methods
    
}