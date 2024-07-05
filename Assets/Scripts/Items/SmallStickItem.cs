using EscapeGuan.Entities;

using UnityEngine;

namespace EscapeGuan.Items
{
    public class SmallStickItem : Item
    {
        public SmallStickItem(string name, string description, Sprite icon) : base(name, description, icon)
        { }

        public override void Use(ItemStack i, Entity from)
        { }

        public override void OnHoldUp(ItemStack i)
        {
            GameManager.Player.AttackDistanceGains.Add("small_stick.attack_distance_gain", .5f);
        }

        public override void OnPutDown(ItemStack i)
        {
            GameManager.Player.AttackDistanceGains.Remove("small_stick.attack_distance_gain");
        }
    }
}
