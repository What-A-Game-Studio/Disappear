using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using WaG;
using WaG.Input_System.Scripts;

public class PlayerInventory : MonoBehaviour
{
    private List<ItemController> itemsInInventory = new List<ItemController>();
    private GameObject player;
    private Transform usableAnchor;
    private GameObject currentUsableGO;
    private Interactable currentUsable;
    private PlayerController pc;

    private GameObject uiGO;
    private InventoryUIManager inventoryUI;
    private Animator inventoryAnimation;

    private static readonly int Close = Animator.StringToHash("Close");
    private static readonly int Open = Animator.StringToHash("Open");

    private float currentWeight;
    private float maxWeight = 50f;

    public void Init(GameObject gameUI, GameObject playerGO)
    {
        currentWeight = 0;
        uiGO = Instantiate(gameUI, transform);
        player = playerGO;
        if (!uiGO.TryGetComponent(out inventoryAnimation))
            Debug.LogError("Could not find Animator Component on GameUI GameObject");

        if (!uiGO.transform.Find("InventoryScreen").Find("BackgroundInventory").Find("Inventory")
            .TryGetComponent(out inventoryUI))
            Debug.LogError("Could not find InventoryUIManager Component on GameUI Children");

        if (!TryGetComponent(out pc))
            Debug.LogError("Could not find PlayerController Script on PlayerController GameObject ");

        usableAnchor = transform.GetChild(0).GetComponent<ModelInfos>().ObjectHolder;

        InputManager.Instance.AddCallbackAction(ActionsControls.OpenInventory, OpenInventory);
        InputManager.Instance.AddCallbackAction(ActionsControls.CloseInventory, CloseInventory);
        InputManager.Instance.AddCallbackAction(ActionsControls.Use, ActivateCurrentUsable);
    }


    private void ActivateCurrentUsable(InputAction.CallbackContext context)
    {
        if (currentUsable != null)
        {
            currentUsable.onInteract?.Invoke(player);
        }
    }

    /// <summary>
    /// Action to open the inventory UI
    /// </summary>
    private void OpenInventory(InputAction.CallbackContext context)
    {
        ChangeInventoryState(100.0f, "Open", CursorLockMode.Confined, true);
    }

    /// <summary>
    /// Action to close the inventory UI
    /// </summary>
    private void CloseInventory(InputAction.CallbackContext context)
    {
        ChangeInventoryState(1.0f, "Close", CursorLockMode.Locked, false);
    }


    /// <summary>
    /// Generic function to update inventory UI display according to player's actions
    /// </summary>
    /// <param name="blurValue"> How much the screen should be blured behind the interface</param>
    /// <param name="animation"> Launch the corresponding animation for opening or closing inventory</param>
    /// <param name="cursorLock"> state of CursorLockMode </param>
    /// <param name="cursorVisible"> Visibility of cursor </param>
    private void ChangeInventoryState(float blurValue, string animation, CursorLockMode cursorLock, bool cursorVisible)
    {
        PostProcessingController.Instance.AdaptBlur(blurValue);
        inventoryAnimation.SetTrigger(animation);
        Cursor.lockState = cursorLock;
        Cursor.visible = cursorVisible;
    }

    /// <summary>
    /// Save an item in the inventory.
    /// If the item is a usable, save its function in currentUsable
    /// </summary>
    /// <param name="item">The item to add</param>
    /// <returns>true if there is there is enough room in the inventory for the item,
    /// false otherwise</returns>
    public bool AddItemToInventory(ItemController item)
    {
        if (item.ItemData.ItemType != ItemType.Usable)
        {
            if (!inventoryUI.StockNewItem(item)) return false;
            itemsInInventory.Add(item);
            currentWeight += item.ItemData.Weight;
            UpdatePlayerWeight();
            return true;
        }

        inventoryUI.StockNewUsable(item, out ItemController previousItem);
        itemsInInventory.Add(item);
        currentWeight += item.ItemData.Weight;
        UpdatePlayerWeight();
        currentUsableGO = ItemManager.Instance.GetUsable(item);
        currentUsableGO = Instantiate(currentUsableGO, usableAnchor.position, Quaternion.identity, usableAnchor);
        if (!currentUsableGO.TryGetComponent(out currentUsable))
        {
            Debug.LogError("Can't find Usable component");
        }

        if (previousItem != null)
            DropItem(previousItem);

        return true;
    }

    /// <summary>
    /// Delete an item from the inventory and respawn its game object in the world
    /// </summary>
    /// <param name="item">The item to drop</param>
    public void DropItem(ItemController item)
    {
        ItemManager.Instance.DropItem(item);
        itemsInInventory.Remove(item);
        currentWeight -= item.ItemData.Weight;
        UpdatePlayerWeight();
    }

    /// <summary>
    /// Verify player weight when the inventory is updated
    /// </summary>
    private void UpdatePlayerWeight()
    {
        if (currentWeight > maxWeight * 0.75f)
            PlayerController.MainPlayer.PlayerWeight = Weight.LargeOverweight;
        
        else if (currentWeight > maxWeight * 0.5f)
            PlayerController.MainPlayer.PlayerWeight = Weight.LigthOverweight;
        
        else
            PlayerController.MainPlayer.PlayerWeight = Weight.Normal;
    }
}