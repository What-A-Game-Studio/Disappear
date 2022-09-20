using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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
    private Transform originalParent;

    private Vector2 mousePosition;
    private Vector2 oldPosition;
    private Image img;

    private bool isMouseOver = false;
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
        if (Input.GetMouseButton(0))
        {
            UpdateMousePosition();
        }
        
        if (Input.GetMouseButtonUp(0))
        {
            img.raycastTarget = true;
        }

        if (Input.GetButtonDown("Discard item") && isMouseOver)
        {
            isMouseOver = false;
            InventoryUIManager.Instance.FreeCases(StoredIndex);
            PlayerController.MainPlayer.PlayerInventory.DropItem(ItemController);
            Destroy(gameObject);
        }
    }

    private void UpdateMousePosition()
    {
        mousePosition.x = Input.mousePosition.x;
        mousePosition.y = Input.mousePosition.y;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        UpdateMousePosition();
        oldPosition = itemTransform.position;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        img.raycastTarget = false;
        transform.SetParent(InventoryUIManager.Instance.itemDraggedContainer, false);
        ;
        InventoryUIManager.Instance.DraggingItem = this;
        InventoryUIManager.Instance.IsDragging = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        itemTransform.position = mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        InventoryUIManager.Instance.DraggingItem = null;
        InventoryUIManager.Instance.IsDragging = false;
        transform.SetParent(originalParent, false);

        if (!canDrop)
        {
            itemTransform.position = oldPosition;
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
            itemTransform.position = oldPosition;
        }

        img.raycastTarget = true;
    }

    public void OnMouseOverCase()
    {
        canDrop = true;
    }

    public void OnMouseExitCase()
    {
        canDrop = false;
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
        if (transform.rotation.z == 0)
            transform.Rotate(0, 0, -90);
        else
            transform.Rotate(0, 0, 90);
        itemTransform.position = mousePosition;
        (itemSize.x, itemSize.y) = (itemSize.y, itemSize.x);
    }
}