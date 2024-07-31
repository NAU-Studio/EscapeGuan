using System.Collections.Generic;

namespace EscapeGuan.Items
{
    public class ItemStackCollection : List<ItemStack>
    {
        public new void Add(ItemStack i)
        {
            base.Add(i);
        }

        public void Set(int index, ItemStack i)
        {
            this[index] = i;
        }

        public new void Remove(ItemStack i)
        {
            this[IndexOf(i)] = null;
        }
    }
}