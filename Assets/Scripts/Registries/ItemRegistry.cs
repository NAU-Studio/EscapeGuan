using System.Collections.Generic;

using EscapeGuan.Entities.Items;

namespace EscapeGuan.Registries
{
    public class ItemRegistry
    {
        private readonly Dictionary<string, Item> Registry = new();

        public static ItemRegistry Main = new();

        private ItemRegistry() { }

        public Item GetObject(string name) => Registry[name];
        public void RegisterObject(string name, Item item) => Registry.Add(name, item);
        public ItemStack CreateItemStack(string name, ItemStackOptions options) => new(Registry[name], options);
    }
}
