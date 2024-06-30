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
        public T GetObject<T>(string name) where T : Item => (T)Registry[name];
        public void RegisterObject(string name, Item item) => Registry.Add(name, item);
        public ItemStack CreateItemStack(string name, int count = 1) => Registry[name].CreateItemStack(count);
    }
}
