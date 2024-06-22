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
        public Dictionary<string, object> Attributes = new();

        public Action<ItemStack> OnRemove = (x) => { };

        public int Count { get => count; set
            {
                if (value <= 0)
                    Delete();
                count = value;
            }
        }
        public float CD => Base.UseCD;

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
