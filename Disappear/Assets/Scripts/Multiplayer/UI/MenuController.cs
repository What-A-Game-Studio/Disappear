using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MenuController : MonoBehaviour
{   
    public string MenuName => menuName;

    [SerializeField] private string menuName;

    public bool IsOpen { get; set; }
    
    public void Open()
    {
        IsOpen = true;
        gameObject.SetActive(IsOpen);   
    }

    public void Close()
    {
        IsOpen = false;
        gameObject.SetActive(IsOpen);   
    }
}
