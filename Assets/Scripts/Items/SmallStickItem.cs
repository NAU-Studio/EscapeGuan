using UnityEngine;

namespace EscapeGuan.Items
{
    public class SmallStickItem : MeleeWeapon
    {
        protected override string ModifierName => "small_stick_item";

        // 这参数是直接传过去的，所以偷懒不命名了
        public SmallStickItem(string a, string b, Sprite c) : base(a, b, c, 32, 1.5f, true, 1.2f, true)
        { }
    }
}
