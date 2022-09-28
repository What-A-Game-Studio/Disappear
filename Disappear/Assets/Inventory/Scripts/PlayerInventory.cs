using System.Collections.Generic;
using UnityEngine;
public class PlayerInventory : MonoBehaviour
{
    private List<ItemController> itemsInInventory = new List<ItemController>();
    private bool inventoryOpened = false;
    private PlayerController pc;

    private GameObject uiGO;
    private InventoryUIManager inventoryUI;
    private Animator inventoryAnimation;

    private static readonly int Close = Animator.StringToHash("Close");
    private static readonly int Open = Animator.StringToHash("Open");

    public void Init(GameObject gameUI)
    {
        uiGO = Instantiate(gameUI, transform);
        if (!uiGO.TryGetComponent(out inventoryAnimation))
            Debug.LogError("Could not find Animator Component on GameUI GameObject");

        if (!uiGO.transform.GetChild(0).GetChild(0).GetChild(0).TryGetComponent(out inventoryUI))
            Debug.LogError("Could not find InventoryUIManager Component on GameUI Children");

        if (!TryGetComponent(out pc))
            Debug.LogError("Could not find PlayerController Script on PlayerController GameObject ");
    }

    private void Update()
    {
        if (!InputManager.Instance.Inventory) return;
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

    private void OpenInventory()
    {
        PostProcessingController.Instance.ActivateBlur();
        inventoryAnimation.SetTrigger(Open);
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        pc.enabled = false;
        pc.CanMoveOrRotate = false;
    }

    private void CloseInventory()
    {
        PostProcessingController.Instance.DeactivateBlur();
        inventoryAnimation.SetTrigger(Close);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        pc.enabled = true;
        pc.CanMoveOrRotate = true;
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
        ItemManager.Instance.DropItem(item);
        itemsInInventory.Remove(item);
    }
}