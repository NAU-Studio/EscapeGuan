using System;
using System.Collections.Generic;

using EscapeGuan.Entities;
using EscapeGuan.Entities.Items;

using UnityEngine;

using Object = UnityEngine.Object;

namespace EscapeGuan.Items
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

        public T GetBase<T>() where T : Item => (T)Base;

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

        public bool Merge(ItemStack i)
        {
            if (i.Base == Base && i.Count + Count <= i.Base.MaxCount)
            {
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

        public void OnHoldUp() => Base.OnHoldUp(this);
        public void OnPutDown() => Base.OnPutDown(this);

        public ItemStack Duplicate(int count) => Base.CreateItemStack(count);


        internal ItemStack(Item b, int c = 1)
        {
            Base = b;
            count = c;
        }
    }
}
