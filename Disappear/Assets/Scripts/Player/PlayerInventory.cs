using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using UnityEngine;
using WebSocketSharp;

public class PlayerInventory : MonoBehaviour
{
    private List<Item> itemsInInventory = new List<Item>();
    private bool inventoryOpened = false;
    private PlayerController pc;

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
        pc.enabled = false;
    }

    private void CloseInventory()
    {
        inventoryAnimation.SetTrigger("Close");
        Cursor.lockState = CursorLockMode.Locked;

        pc.enabled = true;
    }


    public void AddItemToInventory(Item item)
    {
        itemsInInventory.Add(item);
    }

    public void DropItem(Item item)
    {
        PoolSystem.Instance.SpawnItem(item.name, transform.position);
        itemsInInventory.Remove(item);
    }
}