using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
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
        draggingItem.OnMouseOverCase(pointerData.pointerEnter.transform.position);
    }

    private void CheckSurroundingCases(int baseCaseIndex)
    {
        overlapCases.Add(listCases[baseCaseIndex]);
        int y = baseCaseIndex / gridWidth;
        int x = baseCaseIndex - (y * gridWidth);
        int itemWidth = draggingItem.ItemSize.x - 1;
        int itemHeight = draggingItem.ItemSize.y - 1;

        if (itemHeight > 0)
        {
            overlapCases.Add(listCases[(y + draggingItem.SelectedPart.y) * gridWidth + x]);
        }

        bool allCaseFree = true;
        foreach (Case c in overlapCases)
        {
            if (c.Occupied)
            {
                allCaseFree = false;
                break;
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
        Debug.Log("Drop Item...");
        PointerEventData pointerData = data as PointerEventData;
        int index = pointerData.pointerEnter.transform.GetSiblingIndex();
        listCases[index].Occupied = true;
        listCases[index].Img.color = Color.white;
        draggingItem.StockInCase(index);
    }

    public void CatchItemOnCase(int index)
    {
        listCases[index].Occupied = false;
    }
}