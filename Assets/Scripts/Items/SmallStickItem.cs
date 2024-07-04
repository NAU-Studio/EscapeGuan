using EscapeGuan.Entities;

using UnityEngine;

namespace EscapeGuan.Items
{
    public class SmallStickItem : Item
    {
        public SmallStickItem(string name, string description, Sprite icon) : base(name, description, icon)
        { }

        public override void Use(ItemStack i, Entity from)
        {
            base.Use(i, from);
            Debug.Log("目前仅用来测试UI");
        }
    }
}
