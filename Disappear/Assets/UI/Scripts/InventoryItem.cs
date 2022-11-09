using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using WAG.Core.Controls;

public class InventoryItem : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerDownHandler,
    IPointerEnterHandler, IPointerExitHandler
{
    private Vector2Int itemSize;

    public Vector2Int ItemSize
    {
        get => itemSize;
        set => itemSize = value;
    }

    public ItemController ItemController { get; set; }

    private RectTransform itemTransform;

    public Vector2 ItemTransform
    {
        set => itemTransform.position = value;
    }

    private Transform originalParent;

    private Vector2 mousePosition;
    private Vector2 oldPosition;
    private Quaternion oldRotation;
    private Image img;

    private bool isMouseOver = false;
    private bool hasRotate;
    public bool canDrop { get; set; }

    public List<int> StoredIndex { get; private set; }

    private void Awake()
    {
        if (!TryGetComponent(out img))
        {
            Debug.Log("Couldn't find component Image on " + gameObject.name);
        }

        if (!TryGetComponent(out itemTransform))
        {
            Debug.Log("Couldn't find component RectTransform on " + gameObject.name);
        }

        originalParent = itemTransform.parent;
        StoredIndex = new List<int>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!InputManager.Instance.Discard || !isMouseOver) return;
        isMouseOver = false;
        InventoryUIManager.Instance.FreeCases(StoredIndex);
        PlayerController.MainPlayer.PlayerInventory.DropItem(ItemController);
        Destroy(gameObject);
    }

    private void UpdateMousePosition()
    {
        mousePosition = Mouse.current.position.ReadValue();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        hasRotate = false;
        Debug.Log("Pointer Down");
        if (ItemController.ItemData.ItemType != ItemType.Usable)
        {
            UpdateMousePosition();
            oldPosition = itemTransform.position;
            oldRotation = itemTransform.rotation;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("Begin Drag");

        if (ItemController.ItemData.ItemType != ItemType.Usable)
        {
            img.raycastTarget = false;
            transform.SetParent(InventoryUIManager.Instance.itemDraggedContainer, false);

            InventoryUIManager.Instance.DraggedItem = this;
            InventoryUIManager.Instance.IsDragging = true;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (ItemController.ItemData.ItemType != ItemType.Usable)
        {
            UpdateMousePosition();
            itemTransform.position = mousePosition;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (ItemController.ItemData.ItemType != ItemType.Usable)
        {
            InventoryUIManager.Instance.DraggedItem = null;
            InventoryUIManager.Instance.IsDragging = false;
            transform.SetParent(originalParent, false);

            if (!canDrop)
            {
                InventoryUIManager.Instance.RestockInCaseOnDragFailed(StoredIndex);
                if (hasRotate)
                {
                    itemTransform.rotation = oldRotation;
                    (itemSize.x, itemSize.y) = (itemSize.y, itemSize.x);
                }

                itemTransform.position = oldPosition;
            }

            img.raycastTarget = true;
        }
    }

    public void StockInCase(List<int> index, Vector2 dropPosition)
    {
        if (canDrop)
        {
            itemTransform.position = dropPosition;
            StoredIndex = index;
        }
        else
        {
            if (hasRotate)
            {
                itemTransform.rotation = oldRotation;
                (itemSize.x, itemSize.y) = (itemSize.y, itemSize.x);
            }

            itemTransform.position = oldPosition;
        }

        img.raycastTarget = true;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isMouseOver = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isMouseOver = false;
    }

    public void RotateItemPositionOnZAxis()
    {
        hasRotate = !hasRotate;
        if (transform.rotation.z == 0)
            transform.Rotate(0, 0, -90);
        else
            transform.Rotate(0, 0, 90);
        (itemSize.x, itemSize.y) = (itemSize.y, itemSize.x);
    }
}