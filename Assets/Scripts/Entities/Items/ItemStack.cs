using UnityEngine;

using EscapeGuan.Registries;
using System;
using Object = UnityEngine.Object;
using System.Collections.Generic;

namespace EscapeGuan.Entities.Items
{
    public class ItemStack
    {
        public Item Base;
        public Dictionary<string, object> Attributes = new();

        public float Durability => Base.GetDurability(this);

        public Action<ItemStack> OnRemove = (x) => { };

        public int Count
        {
            get => count; set
            {
                if (value <= 0)
                    Delete();
                count = value;
            }
        }

        private int count;

        public ItemEntity CreateEntity(GameObject itemTemplate)
        {
            GameObject go = Object.Instantiate(itemTemplate);
            go.GetComponent<ItemEntity>().item = this;
            go.GetComponent<SpriteRenderer>().sprite = Base.Icon;
            return go.GetComponent<ItemEntity>();
        }

        public ItemEntity CreateEntity(Vector3 position, Vector3 rotation)
        {
            GameObject go = Object.Instantiate(GameManager.Templates["item"], position, Quaternion.Euler(rotation));
            go.GetComponent<ItemEntity>().item = this;
            go.GetComponent<SpriteRenderer>().sprite = Base.Icon;
            return go.GetComponent<ItemEntity>();
        }

        public void Use(Entity i) => Base.Use(this, i);

        public bool Combine(ItemStack i)
        {
            if (i.Base == Base && i.Base.GetDurability(i) == Base.GetDurability(this))
            {
                if (i.Count + Count > i.Base.MaxCount)
                    return false;
                Count += i.Count;
                return true;
            }
            else
                return false;
        }

        public string GetCountString() => Count <= 1 ? "" : Count.ToString();

        public void Delete()
        {
            OnRemove(this);
        }

        internal ItemStack(Item b, int c = 1)
        {
            Base = b;
            count = c;
        }
    }
}
