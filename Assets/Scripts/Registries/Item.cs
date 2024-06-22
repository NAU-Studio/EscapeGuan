using EscapeGuan.Entities;
using EscapeGuan.Entities.Items;

using UnityEngine;

namespace EscapeGuan.Registries
{
    public class Item
    {
        public string Name, Description;
        public Sprite Icon;
        public virtual float UseCD => 0;
        public virtual int MaxCount => 64;

        public Item(string name, string description, Sprite icon)
        {
            Name = name;
            Description = description;
            Icon = icon;
        }

        public virtual void Use(ItemStack i, Entity from)
        {
            i.Count--;
        }

        public virtual ItemStack CreateItemStack(int count = 1)
        {
            return new(this, count);
        }

        /// <summary>
        /// 获取耐久率，单位为“1”。
        /// </summary>
        public virtual float GetDurability(ItemStack i)
        {
            return 1;
        }

        public virtual string GetDescription(ItemStack i)
        {
            return Description;
        }
    }
}
