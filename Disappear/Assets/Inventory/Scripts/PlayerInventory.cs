using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using UnityEngine;
using WebSocketSharp;

public class PlayerInventory : MonoBehaviour
{
    private List<ItemController> itemsInInventory = new List<ItemController>();
    private GameObject player;
    private Transform usableAnchor;
    private GameObject currentUsableGO;
    private Interactable currentUsable;
    private bool inventoryOpened = false;
    private PlayerController pc;

    private GameObject uiGO;
    private InventoryUIManager inventoryUI;
    private Animator inventoryAnimation;

    private static readonly int Close = Animator.StringToHash("Close");
    private static readonly int Open = Animator.StringToHash("Open");

    public void Init(GameObject gameUI, GameObject playerGO)
    {
        uiGO = Instantiate(gameUI, transform);
        player = playerGO;
        if (!uiGO.TryGetComponent(out inventoryAnimation))
            Debug.LogError("Could not find Animator Component on GameUI GameObject");

        if (!uiGO.transform.Find("InventoryScreen").Find("BackgroundInventory").Find("Inventory")
            .TryGetComponent(out inventoryUI))
            Debug.LogError("Could not find InventoryUIManager Component on GameUI Children");

        if (!TryGetComponent(out pc))
            Debug.LogError("Could not find PlayerController Script on PlayerController GameObject ");

        usableAnchor = transform.Find("UsableAnchor");
    }

    private void Update()
    {
        if (Input.GetButtonDown("Inventory"))
        {
            if (!inventoryOpened)
            {
                OpenInventory();
                inventoryOpened = true;
            }
            else
            {
                CloseInventory();
                inventoryOpened = false;
            }
        }

        if (Input.GetButtonDown("ActivateItem") && currentUsable != null)
        {
            currentUsable.onInteract?.Invoke(player);
        }
    }

    /// <summary>
    /// Display the UI for the inventory
    /// </summary>
    private void OpenInventory()
    {
        PostProcessingController.Instance.ActivateBlur();
        inventoryAnimation.SetTrigger(Open);
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        pc.enabled = false;
        pc.CameraController.CanRotate = false;
    }

    /// <summary>
    /// Close the UI of the inventory 
    /// </summary>
    private void CloseInventory()
    {
        PostProcessingController.Instance.DeactivateBlur();
        inventoryAnimation.SetTrigger(Close);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        pc.enabled = true;
        pc.CameraController.CanRotate = true;
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
            return true;
        }

        inventoryUI.StockNewUsable(item, out ItemController previousItem);
        itemsInInventory.Add(item);
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
    }
}