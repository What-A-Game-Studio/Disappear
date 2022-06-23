using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerDownHandler,
    IPointerEnterHandler, IPointerExitHandler
{
    public Vector2Int ItemSize => ItemController.ItemData.Size;
    public ItemController ItemController { get; set; }
    private Vector2Int selectedPart;

    public Vector2Int SelectedPart
    {
        get { return selectedPart; }
    }

    private RectTransform itemTransform;
    private Transform originalParent;

    private Vector2 mousePosition;
    private Vector2 startPosition;
    private Vector2 differencePoint;
    private Vector2 oldPosition;
    private Vector2 selectedPosition;
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

        if (Input.GetMouseButtonDown(0))
        {
            UpdateStartPosition();
            UpdateDifferencePoint();
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

    private void UpdateStartPosition()
    {
        startPosition.x = itemTransform.position.x;
        startPosition.y = itemTransform.position.y;
    }

    private void UpdateDifferencePoint()
    {
        differencePoint = mousePosition - startPosition;
    }


    public void OnPointerDown(PointerEventData eventData)
    {
        UpdateMousePosition();
        selectedPosition = itemTransform.InverseTransformPoint(mousePosition);
        selectedPosition.x += itemTransform.sizeDelta.x / 2;
        selectedPosition.y += itemTransform.sizeDelta.y / 2;
        selectedPart.x = Mathf.FloorToInt(selectedPosition.x / 100);
        selectedPart.y = ItemController.ItemData.Size.y - Mathf.FloorToInt(selectedPosition.y / 100) - 1;
        oldPosition = itemTransform.position;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        img.raycastTarget = false;
        transform.SetParent(InventoryUIManager.Instance.itemDraggedContainer, false);;
        InventoryUIManager.Instance.DraggingItem = this;
        InventoryUIManager.Instance.IsDragging = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        itemTransform.position = mousePosition - differencePoint;
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

    public void RotateItemPositionOnZAxis(int zRotation)
    {
        transform.Rotate(0, 0, zRotation);
    }
}