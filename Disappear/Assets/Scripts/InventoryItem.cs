using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerDownHandler
{
    [field: SerializeField] public Vector2Int ItemSize { get; private set; }

    private Vector2Int selectedPart;

    public Vector2Int SelectedPart
    {
        get { return selectedPart; }
    }

    private RectTransform itemTransform;

    private Vector2 mousePosition;
    private Vector2 startPosition;
    private Vector2 differencePoint;
    private Vector2 oldPosition;
    private Vector2 selectedPosition;
    private Image img;
    public bool canDrop { get; set; }

    public List<int> storedIndex { get; private set; }

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

        storedIndex = new List<int>();
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
        selectedPart.x = ItemSize.x - Mathf.FloorToInt(selectedPosition.x / 100) - 1;
        selectedPart.y = ItemSize.y - Mathf.FloorToInt(selectedPosition.y / 100) - 1;
        Debug.Log("selected part : " + selectedPart);
        oldPosition = itemTransform.position;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        img.raycastTarget = false;
        InventoryUIManager.draggingItem = this;
        InventoryUIManager.isDragging = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        itemTransform.position = mousePosition - differencePoint;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("Henlo : " + canDrop);

        if (!canDrop)
        {
            itemTransform.position = oldPosition;
        }
    }

    public void StockInCase(List<int> index, Vector2 dropPosition)
    {
        Debug.Log("Stock ? " + canDrop);
        if (canDrop)
        {
            itemTransform.position = dropPosition;
            storedIndex = index;
        }
        else
        {
            itemTransform.position = oldPosition;
        }

        InventoryUIManager.draggingItem = null;
        InventoryUIManager.isDragging = false;

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
}