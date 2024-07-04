using System;

using EscapeGuan.Entities.Items;

using UnityEngine;

namespace EscapeGuan.Entities.Props
{
    public abstract class PropEntity : Entity
    {
        // Unit: second
        public float CollisionCD;

        public override bool GuanAttackable => false;
        public override int InventoryLength => throw new Exception($"{Id} has no inventory!");
        public override Vector3 HealthBarOffset => new(0, 0.75f, 0);


        private float CollisionRemaining = 0;

        private void Update()
        {
            if (CollisionRemaining > 0)
                CollisionRemaining -= Time.deltaTime;
        }

        protected virtual void OnCollisionStay2D(Collision2D collision)
        {
            if (CollisionRemaining <= 0)
            {
                Damage(collision.rigidbody.velocity.magnitude * collision.rigidbody.mass);
                CollisionRemaining = CollisionCD;
            }
        }

        public override void PickItem(ItemEntity sender)
        {
            throw new EntityCannotPickupException(Id);
        }
    }
}
