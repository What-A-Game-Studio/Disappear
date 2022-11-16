using UnityEngine;

namespace WAG.Core.Menus
{
    public class MenuLoader : MonoBehaviour
    {
        [SerializeField] private MenuType menu;

        void Start()
        {
            MenuManager.Instance.OpenMenu(menu);
        }
    }
}