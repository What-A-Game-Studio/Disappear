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
    private List<Case> listCases = new List<Case>();
    private List<Case> overlapCases = new List<Case>();

    public static InventoryItem draggingItem;
    public static bool isDragging;

    [SerializeField] private RectTransform itemContainer;
    [SerializeField] private GameObject itemUIPrefab;

    private bool previousState;

    private GridLayoutGroup grid;
    private int gridWidth;
    private int gridHeight;

    // Start is called before the first frame update
    void Awake()
    {
        isDragging = false;
        previousState = isDragging;
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
        if (isDragging && isDragging != previousState)
        {
            CatchItemOnCase(draggingItem.storedIndex);
        }

        previousState = isDragging;
    }

    public void ColorOnMouseOverCase(BaseEventData data)
    {
        if (!isDragging) return;

        PointerEventData pointerData = data as PointerEventData;
        int index = pointerData.pointerEnter.transform.GetSiblingIndex();
        CheckSurroundingCases(index);
        draggingItem.OnMouseOverCase();
    }

    public void StockNewItem(ItemDataSO itemData)
    {
        Sprite itemSprite = Sprite.Create(itemData.Image,
            new Rect(0.0f, 0.0f, itemData.Image.width, itemData.Image.height), new Vector2(0.5f, 0.5f));
        GameObject itemUI = Instantiate(itemUIPrefab, itemContainer);
        itemUI.GetComponent<Image>().sprite = itemSprite;
        InventoryItem uiInventory = itemUI.GetComponent<InventoryItem>();
        uiInventory.ItemSize = itemData.Size;

        for (int i = 0; i < listCases.Count; i++)
        {
            if (listCases[i].Occupied) continue;

            if (itemData.Size.x <= 1 && itemData.Size.y <= 1)
            {
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

        for (int i = y; i < y + itemSize.y - 1; i++)
        {
            if (i < 0 || i > gridHeight)
            {
                overlapCases.Clear();
                return false;
            }

            for (int j = x; j < x + itemSize.x - 1; j++)
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

        int start = y - draggingItem.SelectedPart.y;
        int end = start + draggingItem.ItemSize.y;
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
        if (!isDragging) return;

        PointerEventData pointerData = data as PointerEventData;
        int index = pointerData.pointerEnter.transform.GetSiblingIndex();
        foreach (Case c in overlapCases)
        {
            c.Img.color = Color.white;
        }

        overlapCases.Clear();
        draggingItem.OnMouseExitCase();
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
            CalculateAveragePosition(draggingItem);
        }
        else
        {
            draggingItem.canDrop = false;
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
    }
}