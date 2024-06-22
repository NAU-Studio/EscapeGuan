using System;

using EscapeGuan.Entities.Items;
using EscapeGuan.Items;

namespace EscapeGuan.Entities.Bullet
{
    public class WaterBottleBullet : Bullet
    {
        public override bool GuanAttackable => false;
        public override int InventoryLength => throw new Exception($"{Id} has no inventory!");

        public ItemStack Base;

        public override void Start()
        {
            base.Start();
            Rigidbody.mass = (float)Base.Attributes[WaterBottleItem.Mass];
        }
    }
}
