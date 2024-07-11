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

        public override void Use(ItemStack i, Entity sender)
        {
            if (sender.Id == GameManager.ControlledId)
                Drink(i);
        }

        public void Drink(ItemStack i)
        {
            int amount = Mathf.Min(DurabilityOf(i), Random.Range(45, 120));
            i.Attributes[Durability] = DurabilityOf(i) - amount;
            if (DurabilityOf(i) <= 0)
            {
                Break(i);
                GameManager.Player.AddItem(ItemRegistry.Main.CreateItemStack("empty_bottle"));
            }
            GameManager.Main.ItemProfile.SetText(Name, GetDescription(i));
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
