using System;
using UnityEngine;

namespace EscapeGuan.Entities.Items
{
    public class ItemEntity : Entity
    {
        public ItemStack item;

        public bool GuanPickable;

        public override bool GuanAttackable => GuanPickable;
        public override int InventoryLength => throw new Exception($"{Id} has no inventory!");

        public override bool BulletHitable => false;
        public override bool ShowHealthBarAtTop => false;

        public override void RegisterEntity()
        {
            base.RegisterEntity();
            GameManager.ItemEntities.Add(Id);
        }

        public void Pickup(Entity e)
        {
            GameManager.ItemEntities.Remove(Id);
            e.PickItem(this);
            Kill();
        }

        public override void PickItem(ItemEntity sender)
        {
            throw new EntityCannotPickupException(Id);
        }

        public void Update()
        {
            if (Vector3.Distance(GameManager.Player.transform.position, transform.position) <= GameManager.Player.ItemPickupRange && !GameManager.Player.NearItems.Contains(Id))
                GameManager.Player.AddNear(Id);
            if (Vector3.Distance(GameManager.Player.transform.position, transform.position) > GameManager.Player.ItemPickupRange && GameManager.Player.NearItems.Contains(Id))
                GameManager.Player.RemoveNear(Id);
        }

        public override float GetDamageAmount(float basev) => 0;

        protected override void Damage(float amount) { }

        public override void Damage(float amount, Entity sender) { }
    }
}