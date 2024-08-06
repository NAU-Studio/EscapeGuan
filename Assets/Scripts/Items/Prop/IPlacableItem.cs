using EscapeGuan.Entities;

using UnityEngine;

namespace EscapeGuan.Items.Prop
{
    public interface IPlacableItem
    {
        public GameObject Template { get; }

        public void Place(ItemStack i, Entity from, Vector2 pos);
    }
}