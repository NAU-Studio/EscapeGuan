using EscapeGuan.Entities;
using UnityEngine;

namespace EscapeGuan.Items
{
    public abstract class MeleeWeapon : DurabilityItem, IPlayerModifierItem
    {
        public override int MaxCount => 1;

        public override int MaxDurability => maxDurability;
        private readonly int maxDurability;
        private readonly Modifier atkModifier, disModifier;

        protected abstract string ModifierName { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name">Item display name</param>
        /// <param name="description">Item description</param>
        /// <param name="icon">Item icon</param>
        /// <param name="maxDurability">Max durability</param>
        /// <param name="atkGain">Attack gain</param>
        /// <param name="atkMultiply">Is attack gain multiply</param>
        /// <param name="disGain">Attack distance gain</param>
        /// <param name="disMultiply">Is attack distance gain multiply</param>
        public MeleeWeapon(string name, string description, Sprite icon, int maxDurability, float atkGain, bool atkMultiply, float disGain, bool disMultiply) : base(name, description, icon)
        {
            this.maxDurability = maxDurability;
            atkModifier = new(atkGain, atkMultiply);
            disModifier = new(disGain, disMultiply);
        }

        public override void OnHoldUp(ItemStack i)
        {
            GameManager.Player.AttackValue.Add(ModifierName, atkModifier);
            GameManager.Player.AttackDistance.Add(ModifierName, disModifier);
        }

        public override void OnPutDown(ItemStack i)
        {
            GameManager.Player.AttackValue.Remove(ModifierName);
            GameManager.Player.AttackDistance.Remove(ModifierName);
        }
    }
}
