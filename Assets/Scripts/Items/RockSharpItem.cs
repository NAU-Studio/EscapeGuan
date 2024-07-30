using UnityEngine;

namespace EscapeGuan.Items
{
    public class RockSharpItem : Item, IPlayerModifierItem
    {
        public RockSharpItem(string name, string description, Sprite icon) : base(name, description, icon)
        { }

        public override void OnHoldUp(ItemStack i)
        {
            GameManager.Player.AttackValue.Add("rock_sharp_item", new(2.5f, true));
        }

        public override void OnPutDown(ItemStack i)
        {
            GameManager.Player.AttackValue.Remove("rock_sharp_item");
        }
    }
}