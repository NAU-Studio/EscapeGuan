using EscapeGuan.Entities.Items;
using EscapeGuan.Entities;
using EscapeGuan.Registries;

using UnityEngine;
using EscapeGuan;

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
        if (from.EntityId == GameManager.Main.ControlledEntityId)
            GameManager.Main.WaterUseUI.Exhale(sender);
    }

    public void Drink(ItemStack sender)
    {
        float amount = Random.Range(0.045f, 0.08f);
        sender.Attributes[Mass] = (float)sender.Attributes[Mass] - amount;
        Debug.Log($"喝了 {amount} kg (L)，使用后含量：{sender.Attributes[Mass]} kg (L)");
    }

    public override ItemStack CreateItemStack()
    {
        ItemStack i = new(this);
        i.Attributes.Add(Mass, MaxMass);
        return i;
    }

    public override float GetDurability(ItemStack i)
    {
        return (float)i.Attributes[Mass] / MaxMass;
    }
}