using EscapeGuan.Entities;

using UnityEngine;

namespace EscapeGuan.Items.Prop
{
    public class PlanksProp : Item, IPlacableItem
    {
        public GameObject Template => GameManager.Templates["planks"];

        public PlanksProp(string name, string desc, Sprite spr) : base(name, desc, spr) { }

        public override void Use(ItemStack i, Entity from)
        {
            Place(i, from, GameManager.Player.GetPlacePosition());
            base.Use(i, from);
        }

        public void Place(ItemStack i, Entity sender, Vector2 pos)
        {
            Object.Instantiate(Template, pos, Quaternion.identity);
        }
    }
}
