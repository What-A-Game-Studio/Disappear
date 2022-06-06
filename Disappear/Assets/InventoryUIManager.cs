using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Case
{
    public Image Img { get; set; }
    public bool Occupied { get; set; }

    public Case(Image newImg)
    {
        Img = newImg;
        Occupied = false;
    }
}


public class InventoryUIManager : MonoBehaviour
{
    public static InventoryUIManager Instance { get; set; }
    
    private List<Case> listCases = new List<Case>();
    private List<Case> overlapCases = new List<Case>();

    public InventoryItem DraggingItem { get; set; }
    public bool IsDragging { get; set; }

    [SerializeField] private RectTransform itemContainer;
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
            if (transform.GetChild(i).TryGetComponent(out Image img))
            {
                Case newCase = new Case(img);
                listCases.Add(newCase);
            }
        }

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
            CatchItemOnCase(DraggingItem.StoredIndex);
        }

        previousState = IsDragging;
    }

    public void ColorOnMouseOverCase(BaseEventData data)
    {
        if (!IsDragging) return;

        PointerEventData pointerData = data as PointerEventData;
        int index = pointerData.pointerEnter.transform.GetSiblingIndex();
        CheckSurroundingCases(index);
        DraggingItem.OnMouseOverCase();
    }

    public void StockNewItem(ItemDataSO itemData)
    {
        GameObject itemUI = Instantiate(itemUIPrefab, itemContainer);
        itemUI.GetComponent<Image>().sprite = itemData.Image;
        InventoryItem uiInventory = itemUI.GetComponent<InventoryItem>();
        uiInventory.ItemSize = itemData.Size;
        itemUI.GetComponent<RectTransform>().sizeDelta =
            new Vector2(100 * uiInventory.ItemSize.x, 100 * uiInventory.ItemSize.y);

        for (int i = 0; i < listCases.Count; i++)
        {
            if (listCases[i].Occupied) continue;

            if (itemData.Size.x <= 1 && itemData.Size.y <= 1)
            {
                Debug.Log("Little Item");
                overlapCases.Add(listCases[i]);
                uiInventory.canDrop = true;
                CalculateAveragePosition(uiInventory);
                break;
            }

            if (CheckSurroundingCasesNew(i, itemData.Size))
            {
                uiInventory.canDrop = true;
                CalculateAveragePosition(uiInventory);
                break;
            }
        }
    }

    private bool CheckSurroundingCasesNew(int baseCaseIndex, Vector2Int itemSize)
    {
        overlapCases.Add(listCases[baseCaseIndex]);
        int y = baseCaseIndex / gridWidth;
        int x = baseCaseIndex - (y * gridWidth);

        for (int i = y; i < y + itemSize.y; i++)
        {
            if (i < 0 || i > gridHeight)
            {
                overlapCases.Clear();
                return false;
            }

            for (int j = x; j < x + itemSize.x; j++)
            {
                if (j < 0 || j > gridWidth)
                {
                    overlapCases.Clear();
                    return false;
                }

                Case checkingCase = listCases[i * gridWidth + j];
                if (checkingCase.Occupied)
                {
                    overlapCases.Clear();
                    return false;
                }

                if (!overlapCases.Contains(checkingCase))
                {
                    overlapCases.Add(checkingCase);
                }
            }
        }

        return true;
    }

    private void CheckSurroundingCases(int baseCaseIndex)
    {
        overlapCases.Add(listCases[baseCaseIndex]);
        int y = baseCaseIndex / gridWidth;
        int x = baseCaseIndex - (y * gridWidth);
        bool allCaseFree = true;

        int start = y - DraggingItem.SelectedPart.y;
        int end = start + DraggingItem.ItemSize.y;
        for (int i = start; i < end; i++)
        {
            if (i >= 0 && i < gridHeight)
            {
                Case checkingCase = listCases[i * gridWidth + x];
                if (!overlapCases.Contains(checkingCase))
                {
                    overlapCases.Add(checkingCase);
                    if (checkingCase.Occupied)
                    {
                        allCaseFree = false;
                    }
                }
            }
            else
            {
                allCaseFree = false;
            }
        }

        if (allCaseFree)
        {
            foreach (Case c in overlapCases)
            {
                c.Img.color = Color.green;
            }
        }
        else
        {
            foreach (Case c in overlapCases)
            {
                c.Img.color = Color.red;
            }
        }
    }

    public void ColorOnMouseExitCase(BaseEventData data)
    {
        if (!IsDragging) return;

        PointerEventData pointerData = data as PointerEventData;
        int index = pointerData.pointerEnter.transform.GetSiblingIndex();
        foreach (Case c in overlapCases)
        {
            c.Img.color = Color.white;
        }

        overlapCases.Clear();
        DraggingItem.OnMouseExitCase();
    }
    
    public void DropItemOnCase(BaseEventData data)
    {
        bool allCaseFree = true;
        foreach (Case c in overlapCases)
        {
            if (c.Occupied || c.Img.color == Color.red)
            {
                allCaseFree = false;
            }

            c.Img.color = Color.white;
        }

        if (allCaseFree)
        {
            CalculateAveragePosition(DraggingItem);
        }
        else
        {
            DraggingItem.canDrop = false;
        }

        overlapCases.Clear();
    }

    public void CatchItemOnCase(List<int> index)
    {
        if (index.Count <= 0) return;
        for (int i = 0; i < index.Count; i++)
        {
            listCases[index[i]].Occupied = false;
        }
    }

    private void CalculateAveragePosition(InventoryItem item)
    {
        List<int> index = new List<int>();
        Vector3 itemNewPosition = Vector3.zero;

        foreach (Case c in overlapCases)
        {
            index.Add(listCases.IndexOf(c));
            c.Occupied = true;
            itemNewPosition += c.Img.transform.position;
        }

        itemNewPosition /= overlapCases.Count;
        item.StockInCase(index, itemNewPosition);
        overlapCases.Clear();
    }

    
    /// <summary>
    /// Set to false Case.Occupied at index
    /// </summary>
    /// <param name="indexToFree"></param>
    public void FreeCases(List<int> indexToFree)
    {
        if(indexToFree != null)
            foreach (int idx in indexToFree)
            {
                if(idx < listCases.Count)
                    listCases[idx].Occupied = false;
            }
    }
}