using UnityEngine;

namespace EscapeGuan.Items
{
    public class SmallStickItem : DurabilityItem, IPlayerGainItem
    {
        public override int MaxCount => 1;

        public override int MaxDurability => 32;

        public SmallStickItem(string name, string description, Sprite icon) : base(name, description, icon)
        { }

        public override void OnHoldUp(ItemStack i)
        {
            GameManager.Player.AttackDistanceGains.Add("small_stick.attack_distance_gain", .5f);
            GameManager.Player.AttackDamageGains.Add("small_stick.attack_damage_gain", .2f);
        }

        public override void OnPutDown(ItemStack i)
        {
            GameManager.Player.AttackDistanceGains.Remove("small_stick.attack_distance_gain");
            GameManager.Player.AttackDamageGains.Remove("small_stick.attack_damage_gain");
        }
    }
}
