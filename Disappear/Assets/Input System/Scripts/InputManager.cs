using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using n;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    private PlayerInput playerInput;

    public Vector2 Move { get; private set; }
    public Vector2 Look { get; private set; }
    public bool Run { get; private set; }
    public bool Jump { get; private set; }
    public bool Crouch { get; private set; }
    public bool Inventory { get; private set; }
    public bool Interact { get; private set; }
    public bool Catch { get; private set; }

    private InputActionMap currentMap;
    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction runAction;
    private InputAction jumpAction;
    private InputAction couchAction;
    private InputAction inventoryAction;
    private InputAction interactAction;
    private InputAction catchAction;

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
        currentMap = playerInput.currentActionMap;
        moveAction = currentMap.FindAction("Move");
        lookAction = currentMap.FindAction("Look");
        runAction = currentMap.FindAction("Run");
        jumpAction = currentMap.FindAction("Jump");
        couchAction = currentMap.FindAction("Crouch");
        inventoryAction = currentMap.FindAction("Inventory");
        interactAction = currentMap.FindAction("Interact");
        catchAction = currentMap.FindAction("Catch");

        moveAction.performed += OnMove;
        lookAction.performed += OnLook;
        runAction.performed += OnRun;
        jumpAction.performed += OnJump;
        couchAction.performed += OnCrouch;
        inventoryAction.performed += OnInventory;
        interactAction.performed += OnInteract;
        catchAction.performed += OnCatch;

        moveAction.canceled += OnMove;
        lookAction.canceled += OnLook;
        runAction.canceled += OnRun;
        jumpAction.canceled += OnJump;
        couchAction.canceled += OnCrouch;
        inventoryAction.canceled += OnInventory;
        interactAction.canceled += OnInteract;
        catchAction.canceled += OnCatch;
    }

    private void OnEnable()
    {
        currentMap.Enable();
    }

    private void OnDisable()
    {
        currentMap.Disable();
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

    private void OnInventory(InputAction.CallbackContext context)
    {
        Inventory = context.ReadValueAsButton();
    }

    private void OnInteract(InputAction.CallbackContext context)
    {
        Interact = context.ReadValueAsButton();
    }

    private void OnCatch(InputAction.CallbackContext context)
    {
        Catch = context.ReadValueAsButton();
    }
}