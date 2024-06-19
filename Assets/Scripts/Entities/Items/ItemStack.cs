using UnityEngine;

using EscapeGuan.Registries;
using System;
using Random = UnityEngine.Random;
using Object = UnityEngine.Object;
using System.Collections.Generic;

namespace EscapeGuan.Entities.Items
{
    public class ItemStack : IDisposable
    {
        public Item Base;
        public Sprite Icon => Base.Icon;
        public Dictionary<string, object> Attributes = new();

        public int Count { get => count; set
            {
                if (value <= 0)
                    Delete();
                count = value;
            }
        }
        public float CD => Base.UseCD;


        public Action<ItemStack> OnRemove = (x) => { };

        private int count;

        public ItemEntity CreateEntity(GameObject ItemTemplate)
        {
            GameObject go = Object.Instantiate(ItemTemplate);
            go.GetComponent<ItemEntity>().item = this;
            go.GetComponent<SpriteRenderer>().sprite = Base.Icon;
            return go.GetComponent<ItemEntity>();
        }

        public ItemEntity CreateEntity(Vector3 Position)
        {
            GameObject go = Object.Instantiate(GameManager.Templates["item"], Position, Quaternion.Euler(0, 0, Random.Range(0, 360)));
            go.GetComponent<ItemEntity>().item = this;
            go.GetComponent<SpriteRenderer>().sprite = Base.Icon;
            return go.GetComponent<ItemEntity>();
        }

        public void Use(Entity sender)
        {
            Base.Use(this, sender);
        }

        public bool Combine(ItemStack item)
        {
            if (item.Base == Base && item.Base.GetDurability(item) == Base.GetDurability(this))
            {
                if (item.Count + Count >= item.Base.MaxCount)
                    return false;
                Count += item.Count;
                return true;
            }
            else
                return false;
        }

        public string GetCountString()
        {
            if (Count <= 1)
                return "";
            else
                return Count.ToString();
        }

        public void Delete()
        {
            OnRemove(this);
            Dispose();
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        internal ItemStack(Item b, int c = 1)
        {
            Base = b;
            count = c;
        }
    }
}
