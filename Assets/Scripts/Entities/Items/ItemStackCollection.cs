using System.Collections.Generic;

namespace EscapeGuan.Entities.Items
{
	public class ItemStackCollection : List<ItemStack>
	{
		public new void Add(ItemStack i)
		{
			base.Add(i);
			if (i != null)
				i.OnRemove += Remove;
		}

		public void Set(int index, ItemStack i)
		{
			this[index] = i;
            if (i != null)
                i.OnRemove += Remove;
        }

		public new void Remove(ItemStack i)
		{
			this[IndexOf(i)] = null;
		}
	}
}