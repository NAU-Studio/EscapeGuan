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
            Rigidbody2D rig = Base.CreateEntity(transform.position, transform.rotation.eulerAngles).GetComponent<Rigidbody2D>();
            rig.velocity = -Rigidbody.velocity;
            base.Drop();
        }

        public void Init(float initialvel, ItemStack @base, Entity thrower, float angle)
        {
            InitialVelocity = initialvel;
            Base = @base;
            Thrower = thrower;
            Direction = angle;
        }
    }
}
