using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using UnityEngine;
using WebSocketSharp;

public class PlayerInventory : MonoBehaviour
{
    private List<ItemController> itemsInInventory = new List<ItemController>();
    private bool inventoryOpened = false;
    private PlayerController pc;

    [SerializeField] private InventoryUIManager inventoryUI;
    [SerializeField] private Animator inventoryAnimation;

    private void Awake()
    {
        if (!TryGetComponent(out pc))
        {
            Debug.LogError("Could not find PlayerController Component");
        }
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
    }

    private void OpenInventory()
    {
        inventoryAnimation.SetTrigger("Open");
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        pc.enabled = false;
        pc.CameraController.CanRotate = false;
    }

    private void CloseInventory()
    {
        inventoryAnimation.SetTrigger("Close");
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        pc.enabled = true;
        pc.CameraController.CanRotate = true;
    }


    public bool AddItemToInventory(ItemController item)
    {
        if (inventoryUI.StockNewItem(item))
        {
            itemsInInventory.Add(item);
            return true;
        }

        return false;
    }

    public void DropItem(ItemController item)
    {
        item.Activate();
        itemsInInventory.Remove(item);
    }
}