using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    [field: SerializeField] public Vector2Int ItemSize { get; private set; }

    private Vector2Int selectedPart;

    public Vector2Int SelectedPart
    {
        get { return selectedPart; }
    }

    private Vector2 mousePosition;
    private Vector2 startPosition;
    private Vector2 differencePoint;
    private Vector2 oldPosition;
    private Vector2 dropPosition;

    private Image img;
    private bool isOverCase;

    public int storedIndex { get; private set; }

    private void Awake()
    {
        if (!TryGetComponent(out img))
        {
            Debug.Log("Couldn't find component Image on " + gameObject.name);
        }
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
        startPosition.x = transform.position.x;
        startPosition.y = transform.position.y;
    }

    private void UpdateDifferencePoint()
    {
        differencePoint = mousePosition - startPosition;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Vector2 posLocal = transform.InverseTransformPoint(mousePosition);
        selectedPart.x = posLocal.x < 0 ? 1 : -1;
        selectedPart.y = posLocal.y >= 0 ? 1 : -1;
        Debug.Log("SelectedPart : " + selectedPart);

        oldPosition = transform.position;
        img.raycastTarget = false;
        InventoryUIManager.draggingItem = this;
        InventoryUIManager.isDragging = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = mousePosition - differencePoint;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isOverCase)
        {
            transform.position = oldPosition;
        }
    }

    public void StockInCase(int index)
    {
        if (isOverCase)
        {
            transform.position = dropPosition;
            storedIndex = index;
        }

        InventoryUIManager.draggingItem = null;
        InventoryUIManager.isDragging = false;

        img.raycastTarget = true;
    }

    public void OnMouseOverCase(Vector2 position)
    {
        isOverCase = true;
        dropPosition = position;
    }

    public void OnMouseExitCase()
    {
        isOverCase = false;
    }
}