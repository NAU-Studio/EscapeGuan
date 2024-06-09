using UnityEngine;

using EscapeGuan.Registries;

namespace EscapeGuan.Entities.Items
{
    public class ItemStack
    {
        private ItemStackOptions Options;
        private readonly Item Base;

        public string Name
        {
            get
            {
                if (string.IsNullOrEmpty(Options.CustomName))
                    return Base.Name;
                else
                    return Options.CustomName;
            }
            set => Options.CustomName = value;
        }
        public string Description => Base.Description + '\n' + Options.CustomDescription;
        public Sprite Icon => Base.Icon;
        public int Count { get => Options.Count; set
            {
                Options.Count = value;
                if (Options.Count <= 0)
                    Destroy(this);
            }
        }

        public ItemEntity CreateEntity(GameObject ItemTemplate)
        {
            GameObject go = Object.Instantiate(ItemTemplate);
            go.GetComponent<ItemEntity>().item = this;
            go.GetComponent<SpriteRenderer>().sprite = Base.Icon;
            return go.GetComponent<ItemEntity>();
        }

        public ItemEntity CreateEntity(GameObject ItemTemplate, Vector3 Position, float Rotation)
        {
            GameObject go = Object.Instantiate(ItemTemplate, Position, Quaternion.Euler(0, 0, Rotation));
            go.GetComponent<ItemEntity>().item = this;
            go.GetComponent<SpriteRenderer>().sprite = Base.Icon;
            return go.GetComponent<ItemEntity>();
        }

        public void Use(Entity sender)
        {
            Base.Use(this, sender);
        }

        public static void Destroy(ItemStack item)
        {
            item = null;
        }

        public bool Combine(ItemStack item)
        {
            if (item.Base == Base)
            {
                Count += item.Count;
                return true;
            }
            else
                return false;
        }

        internal ItemStack(Item b, ItemStackOptions o)
        {
            Base = b;
            Options = o;
        }
    }

    public struct ItemStackOptions
    {
        public int Count;
        public string CustomName, CustomDescription;

        public ItemStackOptions(int count = 1, string customName = "", string customDescription = "") : this()
        {
            Count = count;
            CustomName = customName;
            CustomDescription = customDescription;
        }
    }
}
