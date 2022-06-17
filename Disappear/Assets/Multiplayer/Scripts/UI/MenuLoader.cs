using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuLoader : MonoBehaviour
{
    [SerializeField] 
    private MenuType menu; 
    void Start()
    {
        MenuManager.Instance.OpenMenu(menu);
    }
}
