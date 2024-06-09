using System.Collections;
using System.Collections.Generic;

using EscapeGuan.Entities;
using EscapeGuan.Entities.Items;
using EscapeGuan.Registries;

using UnityEngine;

public class TestBottleItem : Item
{
    public TestBottleItem(string name, string description, Sprite icon) : base(name, description, icon)
    { }

    public override void Use(ItemStack sender, Entity from)
    {
        sender.Count = 0;
    }
}
