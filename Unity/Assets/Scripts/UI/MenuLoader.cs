using UnityEngine;

public class MenuLoader : MonoBehaviour
{
    [SerializeField] private MenuType menu;

    private void Start()
    {
        MenuManager.Instance.OpenMenu(menu);
    }
}