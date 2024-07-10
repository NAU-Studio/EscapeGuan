using EscapeGuan.Entities;

using UnityEngine;

namespace EscapeGuan.Items
{
    public class WaterBottleItem : DurabilityItem, IThrowableItem
    {
        public override int MaxCount => 1;
        public override int MaxDurability => 550;

        public WaterBottleItem(string name, string description, Sprite icon) : base(name, description, icon)
        { }

        public override void Use(ItemStack sender, Entity from)
        {
            if (from.Id == GameManager.ControlledId)
                Drink(sender);
        }

        public void Drink(ItemStack sender)
        {
            int amount = Mathf.Min(DurabilityOf(sender), Random.Range(45, 120));
            sender.Attributes[Durability] = DurabilityOf(sender) - amount;
            if (DurabilityOf(sender) <= 0)
            {
                Break(sender);
                GameManager.Player.AddItem(ItemRegistry.Main.CreateItemStack("empty_bottle"));
            }
            GameManager.Main.ItemProfile.SetText(Name, GetDescription(sender));
        }

        public override string GetDescription(ItemStack i)
        {
            return $"{Description}剩余量：{DurabilityOf(i)} mL";
        }

        public void Throw(ItemStack i)
        {
            throw new System.NotImplementedException();
        }
    }
}
