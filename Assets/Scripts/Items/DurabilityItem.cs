using EscapeGuan.Entities;
using EscapeGuan.Items;
using UnityEngine;

public class DurabilityItem : Item
{
    public string Durability => $"{GetType().Name}_Durability";
    public virtual int MaxDurability => 1;

    public DurabilityItem(string name, string description, Sprite icon) : base(name, description, icon)
    { }

    public override ItemStack CreateItemStack(int count = 1)
    {
        ItemStack i = base.CreateItemStack(count);
        i.Attributes.Add(Durability, MaxDurability);
        return i;
    }

    public override void Use(ItemStack i, Entity from)
    {
        i.Attributes[Durability] = (int)i.Attributes[Durability] - 1;
        if ((int)i.Attributes[Durability] <= 0)
            Break(i);
    }

    public override float GetDurability(ItemStack i)
    {
        return (float)(int)i.Attributes[Durability] / MaxDurability;
    }

    public void Break(ItemStack i)
    {
        i.Delete();
    }
}