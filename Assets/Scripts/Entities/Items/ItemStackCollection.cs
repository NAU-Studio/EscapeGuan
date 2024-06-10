using System.Collections.Generic;

namespace EscapeGuan.Entities.Items
{
	public class ItemStackCollection : List<ItemStack>
	{
		public new void Add(ItemStack item)
		{
			base.Add(item);
			if (item != null)
				item.OnRemove += Remove;
		}

		public void Set(int index, ItemStack item)
		{
			this[index] = item;
            if (item != null)
                item.OnRemove += Remove;
        }

		public new void Remove(ItemStack item)
		{
			this[IndexOf(item)] = null;
		}
	}
}