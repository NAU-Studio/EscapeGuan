namespace EscapeGuan.Entities.Items
{
    public class ItemEntity : Entity
    {
        public ItemStack item;

        public override bool GuanAttackable => false;

        public override void RegisterEntity()
        {
            base.RegisterEntity();
            GameManager.Main.ItemEntities.Add(EntityId);
        }

        public void Pickup(Entity e)
        {
            e.Inventory.Add(item);
            GameManager.Main.ItemEntities.Remove(EntityId);
            e.PickItem(this);
            Kill();
        }

        public override void PickItem(ItemEntity sender)
        {
            throw new EntityCannotPickupException(EntityId);
        }
    }
}