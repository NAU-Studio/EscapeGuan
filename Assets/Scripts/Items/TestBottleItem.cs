using EscapeGuan.Entities;
using EscapeGuan.Entities.Items;
using EscapeGuan.Registries;

using UnityEngine;

namespace EscapeGuan.Items
{
    public class TestBottleItem : Item
    {
        public TestBottleItem(string name, string description, Sprite icon) : base(name, description, icon)
        { }

        public override void Use(ItemStack sender, Entity from)
        {
            sender.Count--;
            Debug.Log("这个瓶子被用掉了");
        }
    }
}
