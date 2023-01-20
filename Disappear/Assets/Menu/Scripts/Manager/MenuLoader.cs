using UnityEngine;

namespace WAG.Menu
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