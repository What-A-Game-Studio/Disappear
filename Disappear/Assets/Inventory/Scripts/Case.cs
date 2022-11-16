using UnityEngine.UI;

namespace WAG.Inventory
{
    public class Case
    {
        public Image Img { get; set; }
        public bool Occupied { get; set; }
        public InventoryItem itemOnTop;

        public Case(Image newImg)
        {
            Img = newImg;
            Occupied = false;
            itemOnTop = null;
        }
    }
}
