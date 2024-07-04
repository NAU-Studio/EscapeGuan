using System;

using EscapeGuan.Entities.Items;

using UnityEngine;

namespace EscapeGuan.Entities.Props
{
    public abstract class PropEntity : Entity
    {
        public bool Breakable;
        public float BreakForce;

        public override bool GuanAttackable => false;
        public override int InventoryLength => throw new Exception($"{Id} has no inventory!");
        public override Vector3 HealthBarOffset => new(0, 0.75f, 0);

        protected virtual void OnCollisionEnter2D(Collision2D collision)
        {
            Damage(collision.rigidbody.velocity.magnitude * collision.rigidbody.mass);
        }

        public override void PickItem(ItemEntity sender)
        {
            throw new EntityCannotPickupException(Id);
        }
    }
}
