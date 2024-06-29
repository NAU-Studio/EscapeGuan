using EscapeGuan.Entities.Items;
using EscapeGuan.Entities;
using EscapeGuan.Registries;

using UnityEngine;

namespace EscapeGuan.Items
{
    public class EmptyWaterBottleItem : Item, IThrowableItem
    {
        public override int MaxCount => 99;

        public EmptyWaterBottleItem(string name, string description, Sprite icon) : base(name, description, icon)
        { }

        public override void Use(ItemStack i, Entity from)
        { }

        public void Throw(ItemStack i)
        {
            throw new System.NotImplementedException();
        }
    }
}
