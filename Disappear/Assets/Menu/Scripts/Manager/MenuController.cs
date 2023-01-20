
using UnityEngine;

namespace WAG.Menu
{

    public class MenuController : MonoBehaviour
    {
        [field: SerializeField] public MenuType MenuName { get; private set; }
        [field: SerializeField] public MenuType PreviousMenu { get; private set; }

        public bool IsOpen { get; private set; }
        
        public void SetMenuActiveState(bool state)
        {
            IsOpen = state;
            gameObject.SetActive(state);
        }
    }
}