using EscapeGuan.Entities;
using EscapeGuan.Entities.Items;
using EscapeGuan.Registries;

using UnityEngine;

public class SmallStickItem : Item
{
    public SmallStickItem(string name, string description, Sprite icon) : base(name, description, icon)
    { }

    public override void Use(ItemStack sender, Entity from)
    {
        sender.Count--;
        Debug.Log("目前仅用作测试UI");
    }
}
