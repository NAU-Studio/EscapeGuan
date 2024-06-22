using EscapeGuan.Entities;
using EscapeGuan.Entities.Items;
using EscapeGuan.Registries;

using UnityEngine;

namespace EscapeGuan.Items
{
    public class WaterBottleItem : Item
    {
        public override int MaxCount => 1;


        public const float MaxMass = 0.55f;

        public const string Mass = "WaterBottleItem_Mass";

        public WaterBottleItem(string name, string description, Sprite icon) : base(name, description, icon)
        {

        }

        public override void Use(ItemStack sender, Entity from)
        {
            // sender.Count--;
            Debug.Log($"使用前含量：{sender.Attributes[Mass]} kg (L)");
            if (from.Id == GameManager.ControlledEntityId)
                GameManager.Main.WaterBottleManager.Exhale(sender);
        }

        public void Drink(ItemStack sender)
        {
            float amount = Mathf.Min((float)sender.Attributes[Mass], Random.Range(0.045f, 0.08f));
            sender.Attributes[Mass] = (float)sender.Attributes[Mass] - amount;
            if ((float)sender.Attributes[Mass] <= 0)
            {
                Debug.Log($"喝光了，变成空瓶子");
                GameManager.Player.AddItem(ItemRegistry.Main.CreateItemStack("empty_bottle"));
                sender.Delete();
            }
            Debug.Log($"喝了 {amount} kg (L)，使用后含量：{sender.Attributes[Mass]} kg (L)");
            GameManager.Main.ItemProfile.SetText(Name, GetDescription(sender));
        }

        public override ItemStack CreateItemStack(int count = 1)
        {
            ItemStack i = new(this, count);
            i.Attributes.Add(Mass, MaxMass);
            return i;
        }

        public ItemStack CreateItemStack(int count = 1, float mass = MaxMass)
        {
            ItemStack i = new(this, count);
            i.Attributes.Add(Mass, MaxMass);
            return i;
        }

        public override float GetDurability(ItemStack i)
        {
            return (float)i.Attributes[Mass] / MaxMass;
        }

        public override string GetDescription(ItemStack i)
        {
            return $"{Description}剩余量：{(float)i.Attributes[Mass] * 1000:0.00} mL";
        }
    }
}
