using EscapeGuan.Items;

public class InventoryOperationSlot : InventorySlotBase
{
    public override ItemStack Item { get => item; }
    public bool PlayerPlacable;

    private ItemStack item;

    public void Set(ItemStack i = null)
    {
        item = i;
    }
}
