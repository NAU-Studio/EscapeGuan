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

    public virtual ItemStack CreateItemStack(int count = 1, bool random = false)
    {
        ItemStack i = base.CreateItemStack(count);
        if (random)
            i.Attributes.Add(Durability, Random.Range(0, MaxDurability));
        else
            i.Attributes.Add(Durability, MaxDurability);
        return i;
    }

    public override void Use(ItemStack i, Entity from)
    {
        Damage(i, 1);
    }

    public void Damage(ItemStack i, int amount)
    {
        SetDurability(i, DurabilityOf(i) - amount);
        if (DurabilityOf(i) <= 0)
            Break(i);
    }

    public void DamageNoCheck(ItemStack i, int amount)
    {
        SetDurability(i, DurabilityOf(i) - amount);
    }

    public void Repair(ItemStack i, int amount)
    {
        SetDurability(i, DurabilityOf(i) + amount);
        if ((int)i.Attributes[Durability] > MaxDurability)
            i.Attributes[Durability] = MaxDurability;
    }

    public override float GetDurability(ItemStack i)
    {
        return (float)DurabilityOf(i) / MaxDurability;
    }

    public int DurabilityOf(ItemStack i) => (int)i.Attributes[Durability];
    public void SetDurability(ItemStack i, int amount) => i.Attributes[Durability] = amount;

    public void Break(ItemStack i)
    {
        i.Count--;
        i.Attributes[Durability] = MaxDurability;
    }
}