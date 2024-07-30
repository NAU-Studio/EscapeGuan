using UnityEngine;

namespace EscapeGuan.Items
{
    public class SmallStickItem : DurabilityItem, IPlayerModifierItem
    {
        public override int MaxCount => 1;

        public override int MaxDurability => 32;

        public SmallStickItem(string name, string description, Sprite icon) : base(name, description, icon)
        { }

        public override void OnHoldUp(ItemStack i)
        {
            GameManager.Player.AttackValue.Add("small_stick_item", new(1.5f, true));
            GameManager.Player.AttackDistance.Add("small_stick_item", new(1.2f, true));
        }

        public override void OnPutDown(ItemStack i)
        {
            GameManager.Player.AttackValue.Remove("small_stick_item");
            GameManager.Player.AttackDistance.Remove("small_stick_item");
        }
    }
}
