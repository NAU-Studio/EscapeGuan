using System;

using EscapeGuan.Entities.Items;
using EscapeGuan.Items;
using UnityEngine;

namespace EscapeGuan.Entities.Bullet
{
    public class WaterBottleBullet : Bullet
    {
        public override bool GuanAttackable => false;
        public override int InventoryLength => throw new Exception($"{Id} has no inventory!");

        public ItemStack Base;

        public Transform Shadow;

        public override void Start()
        {
            base.Start();
            Rigidbody.mass = (float)Base.Attributes[WaterBottleItem.Mass];
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            Shadow.transform.localScale = new(ZCoord / 2, ZCoord / 2, 1);
        }

        public override void Drop()
        {
            Base.CreateEntity(transform.position, transform.rotation.eulerAngles);
            base.Drop();
        }

        public override void Hit(Entity e)
        {
            Base.CreateEntity(transform.position, transform.rotation.eulerAngles);
            base.Hit(e);
        }
    }
}
