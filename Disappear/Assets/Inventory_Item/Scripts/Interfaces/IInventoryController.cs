namespace WAG.Inventory_Items
{
    public interface IInventoryController
    {
        bool AddItemToInventory(IItemController item);
    }
}