using UnityEngine;

namespace EscapeGuan.Items
{
    public class RockKnifeItem : MeleeWeapon
    {
        protected override string ModifierName => "rock_knife_item";

        public RockKnifeItem(string a, string b, Sprite c) : base(a, b, c, 128, 2.75f, true, 1.15f, true)
        { }
    }
}
