using System.Collections.Generic;

using EscapeGuan.UI.Items;

namespace EscapeGuan.Items.Recipes
{
    public class CraftingInput
    {
        public List<ItemStack> Items = new();
        public int Width, Height;

        public CraftingInput(InventoryOperationSlot[] slots, int w, int h)
        {
            Width = w;
            Height = h;
            foreach (InventoryOperationSlot i in slots)
                Items.Add(i.Item);
        }

        public List<Item> ShapelessList
        {
            get
            {
                List<Item> l = new();
                foreach (ItemStack i in Items)
                    if (i != null)
                        l.Add(i.Base);
                return l;
            }
        }
    }
}
