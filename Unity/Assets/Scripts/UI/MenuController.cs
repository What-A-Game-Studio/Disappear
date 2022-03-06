using UnityEngine;

public class MenuController : MonoBehaviour
{
    [SerializeField] private MenuType menuName;

    public bool IsOpen;
    public MenuType MenuName => menuName;

    public void Open()
    {
        IsOpen = true;
        gameObject.SetActive(true);
    }

    public void Close()
    {
        IsOpen = false;
        gameObject.SetActive(false);
    }
}