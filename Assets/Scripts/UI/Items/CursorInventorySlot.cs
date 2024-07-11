using EscapeGuan.Items;

namespace EscapeGuan.UI.Items
{
    public class CursorInventorySlot : InventorySlotBase
    {
        public override ItemStack Item => item;
        private ItemStack item;

        public override void SetItem(ItemStack i = null)
        {
            item = i;
        }
    }
}
