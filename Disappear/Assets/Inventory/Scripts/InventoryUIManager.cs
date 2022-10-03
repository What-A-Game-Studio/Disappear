using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Case
{
    public Image Img { get; set; }
    public bool Occupied { get; set; }
    public InventoryItem itemOnTop;

    public Case(Image newImg)
    {
        Img = newImg;
        Occupied = false;
        itemOnTop = null;
    }
}


public class InventoryUIManager : MonoBehaviour
{
    public static InventoryUIManager Instance { get; protected set; }

    private List<Case> listCases = new List<Case>();
    private Case usableCase;
    private List<int> overlapCasesIndex = new List<int>();

    public InventoryItem DraggingItem { get; set; }
    public bool IsDragging { get; set; }


    [SerializeField] private RectTransform itemContainer;
    [field: SerializeField] public Transform itemDraggedContainer { get; private set; }
    [SerializeField] private GameObject itemUIPrefab;

    private bool previousState;

    private GridLayoutGroup grid;
    private int gridWidth;
    private int gridHeight;

    // Start is called before the first frame update
    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        IsDragging = false;
        previousState = IsDragging;
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).TryGetComponent(out Image inventoryImage))
            {
                Case newCase = new Case(inventoryImage);
                listCases.Add(newCase);
            }
        }

        if (transform.parent.Find("UsableCase").TryGetComponent(out Image usableImage))
            usableCase = new Case(usableImage);
        else
            Debug.LogError("Couldn't find Image for Usable Case on " + gameObject.name);


        if (!TryGetComponent(out grid))
        {
            Debug.LogError("Couldn't find Grid Layout Group Component on " + gameObject.name);
        }

        gridWidth = grid.constraintCount;
        gridHeight = transform.childCount / gridWidth;
    }

    private void Update()
    {
        if (IsDragging && IsDragging != previousState)
        {
            CatchItemOnCase();
        }
        previousState = IsDragging;
    }

    /// <summary>
    /// Change color of cases when dragging an item over the inventory
    /// </summary>
    /// <param name="data"></param>
    public void ColorOnMouseOverCase(BaseEventData data)
    {
        if (!IsDragging) return;

        PointerEventData pointerData = data as PointerEventData;
        int index = pointerData.pointerEnter.transform.GetSiblingIndex();
        CheckSurroundingCasesOnDragOver(index);
        DraggingItem.OnMouseOverCase();
    }


    /// <summary>
    /// Create corresponding item in the inventory when picking item 
    /// </summary>
    /// <param name="itemController">Informations of picked item</param>
    /// <returns>true if there is enough space for item, false otherwise</returns>
    public bool StockNewItem(ItemController itemController)
    {
        for (int i = 0; i < listCases.Count; i++)
        {
            if (listCases[i].Occupied) continue;

            if (itemController.ItemData.Size.x <= 1 && itemController.ItemData.Size.y <= 1)
            {
                InventoryItem uiInventory = GenerateUIItem(itemController);
                listCases[i].Occupied = true;
                overlapCasesIndex.Add(i);
                CalculateAveragePosition(uiInventory);
                return true;
            }

            if (CheckSurroundingCasesOnStock(i, itemController.ItemData.Size))
            {
                InventoryItem uiInventory = GenerateUIItem(itemController);
                CalculateAveragePosition(uiInventory);
                return true;
            }
        }

        return false;
    }

    public void StockNewUsable(ItemController itemController, out ItemController previousItem)
    {
        if (usableCase.Occupied)
        {
            previousItem = usableCase.itemOnTop.ItemController;
            Destroy(usableCase.itemOnTop.gameObject);
        }
        else
        {
            usableCase.Occupied = true;
            previousItem = null;
        }

        InventoryItem uiInventory = GenerateUIItem(itemController);
        uiInventory.ItemTransform = usableCase.Img.transform.position;
        usableCase.itemOnTop = uiInventory;
    }

    /// <summary>
    /// Create the item UI prefab of picked item 
    /// </summary>
    /// <param name="itemController">Information of picked item</param>
    /// <returns> The InventoryItem component of the created object</returns>
    public InventoryItem GenerateUIItem(ItemController itemController)
    {
        GameObject itemUI = Instantiate(itemUIPrefab, itemContainer);
        itemUI.GetComponent<Image>().sprite = itemController.ItemData.Image;
        InventoryItem uiInventory = itemUI.GetComponent<InventoryItem>();
        uiInventory.ItemController = itemController;
        uiInventory.ItemSize = itemController.ItemData.Size;
        itemUI.GetComponent<RectTransform>().sizeDelta =
            new Vector2(100 * uiInventory.ItemSize.x, 100 * uiInventory.ItemSize.y);
        uiInventory.canDrop = true;
        return uiInventory;
    }

    /// <summary>
    /// Check if surrounding cases are free when trying to pick up a new item
    /// </summary>
    /// <param name="baseCaseIndex">the index to start in the inventory grid</param>
    /// <param name="itemSize">the size of the item we are trying to stock</param>
    /// <returns>true if surronding cases are free, false otherwise</returns>
    private bool CheckSurroundingCasesOnStock(int baseCaseIndex, Vector2Int itemSize)
    {
        overlapCasesIndex.Add(baseCaseIndex);
        int y = baseCaseIndex / gridWidth;
        int x = baseCaseIndex - (y * gridWidth);

        for (int i = y; i < y + itemSize.y; i++)
        {
            if (i < 0 || i >= gridHeight)
            {
                overlapCasesIndex.Clear();
                return false;
            }

            for (int j = x; j < x + itemSize.x; j++)
            {
                if (j < 0 || j >= gridWidth)
                {
                    overlapCasesIndex.Clear();
                    return false;
                }

                int checkingIndex = i * gridWidth + j;
                if (listCases[checkingIndex].Occupied)
                {
                    overlapCasesIndex.Clear();
                    return false;
                }

                if (!overlapCasesIndex.Contains(checkingIndex))
                    overlapCasesIndex.Add(checkingIndex);
            }
        }

        return true;
    }


    /// <summary>
    /// Check if surrounding cases are free when dragging item over a case.
    /// Change color of cases according to their availability
    /// </summary>
    /// <param name="baseCaseIndex">the index of the case currently overing</param>
    private void CheckSurroundingCasesOnDragOver(int baseCaseIndex)
    {
        overlapCasesIndex.Add(baseCaseIndex);
        int y = baseCaseIndex / gridWidth;
        int x = baseCaseIndex - (y * gridWidth);
        bool allCaseFree = true;

        int startY = y - Mathf.FloorToInt(DraggingItem.ItemSize.y / 2);
        int endY = startY + DraggingItem.ItemSize.y;
        int startX = x - Mathf.FloorToInt(DraggingItem.ItemSize.x / 2);
        int endX = startX + DraggingItem.ItemSize.x;


        for (int i = startY; i < endY; i++)
        {
            for (int j = startX; j < endX; j++)
            {
                int index = i * gridWidth + j;

                if (!overlapCasesIndex.Contains(index))
                {
                    if (i >= 0 && i < gridHeight && j >= 0 && j < gridWidth)
                    {
                        overlapCasesIndex.Add(index);
                        if (listCases[index].Occupied)
                            allCaseFree = false;
                    }
                    else
                    {
                        allCaseFree = false;
                    }
                }
            }
        }

        if (allCaseFree)
        {
            foreach (int c in overlapCasesIndex)
                listCases[c].Img.color = Color.green;
        }
        else
        {
            foreach (int c in overlapCasesIndex)
                listCases[c].Img.color = Color.red;
        }
    }

    /// <summary>
    /// Reset color of cases when mouse quit
    /// </summary>
    /// <param name="data"></param>
    public void ColorOnMouseExitCase(BaseEventData data)
    {
        if (!IsDragging) return;

        PointerEventData pointerData = data as PointerEventData;
        int index = pointerData.pointerEnter.transform.GetSiblingIndex();
        foreach (int c in overlapCasesIndex)
        {
            listCases[c].Img.color = Color.white;
        }

        overlapCasesIndex.Clear();
        DraggingItem.OnMouseExitCase();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="data"></param>
    public void DropItemOnCase(BaseEventData data)
    {
        if (DraggingItem != null)
        {
            bool allCaseFree = true;
            foreach (int c in overlapCasesIndex)
            {
                if (listCases[c].Occupied || listCases[c].Img.color == Color.red)
                {
                    allCaseFree = false;
                }

                listCases[c].Img.color = Color.white;
            }

            if (allCaseFree)
            {
                CalculateAveragePosition(DraggingItem);
            }
            else
            {
                DraggingItem.canDrop = false;
            }

            overlapCasesIndex.Clear();
        }
    }

    public void CatchItemOnCase()
    {
        if (DraggingItem.StoredIndex.Count <= 0) return;
        for (int i = 0; i < DraggingItem.StoredIndex.Count; i++)
        {
            listCases[DraggingItem.StoredIndex[i]].Occupied = false;
        }
    }

    private void CalculateAveragePosition(InventoryItem item)
    {
        List<int> index = new List<int>();
        Vector3 itemNewPosition = Vector3.zero;

        foreach (int c in overlapCasesIndex)
        {
            index.Add(listCases.IndexOf(listCases[c]));
            listCases[c].Occupied = true;
            itemNewPosition += listCases[c].Img.transform.position;
        }

        itemNewPosition /= overlapCasesIndex.Count;
        item.StockInCase(index, itemNewPosition);
        overlapCasesIndex.Clear();
    }


    /// <summary>
    /// Set to false Case.Occupied at index
    /// </summary>
    /// <param name="indexToFree"></param>
    public void FreeCases(List<int> indexToFree)
    {
        if (indexToFree.Count > 0)
        {
            foreach (int idx in indexToFree)
            {
                if (idx < listCases.Count)
                    listCases[idx].Occupied = false;
            }
        }
        else
        {
            usableCase.Occupied = false;
        }
    }
}