using EscapeGuan.Items;

namespace EscapeGuan.UI.Items
{
    public class InventorySlot : InventorySlotBase
    {
        public override ItemStack Item => GameManager.Player.Inventory[Index];
        public int Index;

        public void Use()
        {
            Item?.Use(GameManager.Player);
        }

        public override void SetItem(ItemStack i = null)
        {
            GameManager.Player.Inventory[Index] = i;
        }
    }
}
