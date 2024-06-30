using System;

using EscapeGuan.Entities.Items;
using EscapeGuan.Items;
using EscapeGuan.Registries;

using UnityEngine;

using Random = UnityEngine.Random;

namespace EscapeGuan.Entities.Bullet
{
    public class WaterBottleBullet : Bullet
    {
        public override bool GuanAttackable => false;
        public override int InventoryLength => throw new Exception($"{Id} has no inventory!");

        public ItemStack Base;

        public override void Drop()
        {
            if (Random.value < ItemRegistry.Main.GetObject<WaterBottleItem>("water_bottle").MassOf(Base) * Rigidbody.velocity.magnitude / (WaterBottleItem.MaxMass * 2))
            {
                for (int i = 0; i < ItemRegistry.Main.GetObject<WaterBottleItem>("water_bottle").MassOf(Base) * 100; i++)
                {
                    WaterDropBullet b = Instantiate(GameManager.Templates["water_drop"], transform.position, Quaternion.identity).GetComponent<WaterDropBullet>();
                    b.Init(this, Random.Range(1, Rigidbody.velocity.magnitude / 2), Random.Range(0, 360f), Thrower.CriticalRate, Thrower.CriticalMultiplier, GetAttackAmount());
                }
                base.Drop();
                return;
            }

            Rigidbody2D rig = Base.CreateEntity(transform.position, transform.rotation.eulerAngles).GetComponent<Rigidbody2D>();
            rig.velocity = -Rigidbody.velocity;
            base.Drop();
        }

        public void Init(Entity thrower, float initvel, float angle, ItemStack @base)
        {
            Init(thrower, initvel, angle);
            Base = @base;
            Rigidbody.mass = (float)Base.Attributes[WaterBottleItem.Mass];
            transform.eulerAngles = new(0, 0, Random.Range(0f, 360));
            Rigidbody.angularVelocity = Random.Range(45f, 135f);
        }
    }
}
