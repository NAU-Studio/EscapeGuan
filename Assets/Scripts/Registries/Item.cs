﻿using EscapeGuan.Entities;
using EscapeGuan.Entities.Items;

using UnityEngine;

namespace EscapeGuan.Registries
{
    public class Item
    {
        public string Name, Description;
        public Sprite Icon;
        public virtual float UseCD => 0;

        public virtual int MaxCount => 99;

        
        public Item(string name, string description, Sprite icon)
        {
            Name = name;
            Description = description;
            Icon = icon;
        }

        public virtual void Use(ItemStack sender, Entity from)
        {

        }

        public virtual ItemStack CreateItemStack()
        {
            return new(this);
        }

        /// <summary>
        /// 获取耐久率，单位为“1”。
        /// </summary>
        public virtual float GetDurability(ItemStack i)
        {
            return 1;
        }
    }
}
