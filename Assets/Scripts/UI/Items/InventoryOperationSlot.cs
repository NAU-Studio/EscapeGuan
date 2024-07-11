using EscapeGuan.Items;

namespace EscapeGuan.UI.Items
{
    public class InventoryOperationSlot : InventorySlotBase
    {
        public override ItemStack Item => item;
        public bool PlayerPlacable;

        public delegate void OnItemChangedEventHandler();
        public OnItemChangedEventHandler OnItemChanged = () => { };

        private ItemStack item;

        public override void SetItem(ItemStack i = null)
        {
            item = i;
            OnItemChanged();
        }

        public void SetItemSilently(ItemStack i = null)
        {
            item = i;
        }
    }
}
