using System;
using SteamAudio;
using UnityEngine;
using UnityEngine.InputSystem;

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
    public bool OpenInventory { get; private set; }
    public bool Interact { get; private set; }
    public bool Catch { get; private set; }
    public bool Use { get; private set; }

    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction runAction;
    private InputAction jumpAction;
    private InputAction couchAction;
    private InputAction openInventoryAction;
    private InputAction interactAction;
    private InputAction catchAction;
    private InputAction useAction;

    #endregion

    #region UI Controls

    public bool Discard { get; private set; }
    public bool LeftMouse { get; private set; }
    public bool ReleaseLeftMouse { get; private set; }
    public bool Rotate { get; private set; }
    public bool CloseInventory { get; private set; }
    
    private InputAction discardAction;
    private InputAction leftMouseAction;
    private InputAction releaseLeftMouseAction;
    private InputAction rotateAction;
    private InputAction closeInventoryAction;

    #endregion


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
        
        moveAction = playerInput.actions["Move"];
        lookAction = playerInput.actions["Look"];
        runAction = playerInput.actions["Run"];
        jumpAction = playerInput.actions["Jump"];
        couchAction = playerInput.actions["Crouch"];
        openInventoryAction = playerInput.actions["OpenInventory"];
        interactAction = playerInput.actions["Interact"];
        catchAction = playerInput.actions["Catch"];
        useAction = playerInput.actions["Use"];
        discardAction = playerInput.actions["Discard"];
        leftMouseAction = playerInput.actions["LeftMouse"];
        releaseLeftMouseAction = playerInput.actions["ReleaseLeftMouse"];
        rotateAction = playerInput.actions["Rotate"];
        closeInventoryAction = playerInput.actions["CloseInventory"];

        moveAction.performed += OnMove;
        lookAction.performed += OnLook;
        runAction.performed += OnRun;
        jumpAction.performed += OnJump;
        couchAction.performed += OnCrouch;
        openInventoryAction.performed += OnOpenInventory;
        interactAction.performed += OnInteract;
        catchAction.performed += OnCatch;
        useAction.performed += OnUse;
        discardAction.performed += OnDiscard;
        leftMouseAction.performed += OnLeftMousePress;
        releaseLeftMouseAction.performed += OnReleaseLeftMousePress;
        rotateAction.performed += OnRotate;
        closeInventoryAction.performed += OnCloseInventory;

        moveAction.canceled += OnMove;
        lookAction.canceled += OnLook;
        runAction.canceled += OnRun;
        jumpAction.canceled += OnJump;
        couchAction.canceled += OnCrouch;
        openInventoryAction.canceled += OnOpenInventory;
        interactAction.canceled += OnInteract;
        catchAction.canceled += OnCatch;
        useAction.canceled += OnUse;
        discardAction.canceled += OnDiscard;
        leftMouseAction.canceled += OnLeftMousePress;
        releaseLeftMouseAction.performed += OnReleaseLeftMousePress;
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
        OpenInventory = context.ReadValueAsButton();
        playerInput.SwitchCurrentActionMap("UI");
    }
    
    private void OnCloseInventory(InputAction.CallbackContext context)
    {
        CloseInventory = context.ReadValueAsButton();
        playerInput.SwitchCurrentActionMap("Player");
    }

    private void OnInteract(InputAction.CallbackContext context)
    {
        Interact = context.ReadValueAsButton();
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

    private void OnLeftMousePress(InputAction.CallbackContext context)
    {
        LeftMouse = context.ReadValueAsButton();
    }
    
    private void OnReleaseLeftMousePress(InputAction.CallbackContext context)
    {
        ReleaseLeftMouse = context.ReadValueAsButton();
    }
    
    #endregion

    #region Public Methods

    public void AddCallbackAction(string actionName, Action<InputAction.CallbackContext> callback)
    {
        playerInput.actions[actionName].performed += callback;
        playerInput.actions[actionName].canceled += callback;
    }
    

    #endregion
}