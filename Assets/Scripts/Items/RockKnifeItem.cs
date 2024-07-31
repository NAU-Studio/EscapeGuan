using UnityEngine;

namespace EscapeGuan.Items
{
    public class RockKnifeItem : MeleeWeapon
    {
        protected override string ModifierName => "small_stick_item";

        // 这参数是直接传过去的，所以偷懒不命名了
        public RockKnifeItem(string a, string b, Sprite c) : base(a, b, c, 128, 2.75f, true, 1.15f, true)
        { }
    }
}
