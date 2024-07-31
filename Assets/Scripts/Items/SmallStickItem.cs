using EscapeGuan.Entities;
using UnityEngine;

namespace EscapeGuan.Items
{
    public class SmallStickItem : Item
    {
        private readonly Modifier atkModifier = new(1.5f, true), disModifier = new(1.2f, true);

        // 这参数是直接传过去的，所以偷懒不命名了
        public SmallStickItem(string a, string b, Sprite c) : base(a, b, c)
        { }

        public override void OnHoldUp(ItemStack i)
        {
            GameManager.Player.AttackValue.Add("small_stick_item", atkModifier);
            GameManager.Player.AttackDistance.Add("small_stick_item", disModifier);
        }

        public override void OnPutDown(ItemStack i)
        {
            GameManager.Player.AttackValue.Remove("small_stick_item");
            GameManager.Player.AttackDistance.Remove("small_stick_item");
        }
    }
}
