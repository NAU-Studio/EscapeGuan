using System;

using EscapeGuan.Entities.Items;
using EscapeGuan.Items;

using UnityEngine;
using UnityEngine.InputSystem;

namespace EscapeGuan.UI.Item.WaterBottle
{
    public class WaterBottleManager : MonoBehaviour
    {
        ItemStack Exhaler;

        Hidable Hidable => GetComponent<Hidable>();

        public void Exhale(ItemStack sender)
        {
            Exhaler = sender;
            Hidable.Show();
        }

        public void Drink()
        {
            ((WaterBottleItem)Exhaler.Base).Drink(Exhaler);
            Exhaler = null;
            Hidable.Hide();
        }

        public void Throw()
        {
            Hidable.Hide();
            GameManager.Main.Crosshair.FloatingAmount = GameManager.Player.BottleThrowFloating;
        }

        public void ThrowEmpty()
        {
            throw new NotImplementedException();
        }

        private void Update()
        {
            if (Keyboard.current.xKey.wasReleasedThisFrame)
            {
                Drink();
                return;
            }
            if (Keyboard.current.cKey.wasReleasedThisFrame)
            {
                Throw();
                return;
            }
        }
    }
}
