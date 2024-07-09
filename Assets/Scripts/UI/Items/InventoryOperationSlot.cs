using EscapeGuan.Items;

public class InventoryOperationSlot : InventorySlotBase
{
    public override ItemStack Item => item;
    public bool PlayerPlacable;

    private ItemStack item;

    public override void SetItem(ItemStack i = null)
    {
        item = i;
    }
}
