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
        public string Description
        {
            get => Base.Description + '\n' + Options.CustomDescription;
        }

        public void SetCount(int count) => Options.Count = count;

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

        internal ItemStack(Item b)
        {
            Base = b;
            Options = new(1);
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
