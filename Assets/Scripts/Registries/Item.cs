using UnityEngine;

namespace EscapeGuan.Registries
{
    public class Item
    {
        public string Name, Description;
        public Sprite Icon;

        public Item(string name, string description, Sprite icon)
        {
            Name = name;
            Description = description;
            Icon = icon;
        }
    }
}
