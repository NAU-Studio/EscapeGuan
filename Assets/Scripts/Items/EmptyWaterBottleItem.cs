using EscapeGuan.Entities.Items;
using EscapeGuan.Entities;
using EscapeGuan.Registries;

using UnityEngine;
using EscapeGuan;

public class EmptyWaterBottleItem : Item
{
    public override int MaxCount => 99;

    public EmptyWaterBottleItem(string name, string description, Sprite icon) : base(name, description, icon)
    {

    }

    public override void Use(ItemStack sender, Entity from)
    {
        sender.Count--;
        GameManager.Main.WaterBottleManager.ThrowEmpty();
    }
}